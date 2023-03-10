using System;
using Unity.Mathematics;
using UnityEngine;

namespace KOI
{
    public class CentralLargeIslandMapGenerator: WorldGenerator
    {
        private float[,] _gradientMap;
        private WorldMap _worldMap;

        public void Initialize(WorldMap worldMap)
        {
            GenerateCircularGradient();
            _worldMap = worldMap;
        }

        public WorldMap GenerateMap()
        {
            Vector2 Org = new Vector2(Mathf.Sqrt(MapConfig.Seed), Mathf.Sqrt(MapConfig.Seed));
            int id = 0;

            for (int x = 0; x < MapConfig.WorldMapWidth; x++)
            {
                for (int y = 0; y < MapConfig.WorldMapHeight; y++)
                {
                    // get original noise map
                    float a = Noisefunction(x, y, Org, MapConfig.NoiseScale);
                    // clear out the edges to get a central island
                    a = (a * 0.8f) - _gradientMap[x, y];

                    // float a = _gradientMap[x, y];   // Generate gradient mask only

                    // if(a > - 0.2){ // this if clause is required for 2nd mask
                    //     a = Noisefunction(x, y, Org, 0.3f) * 0.8f; // this is required for 2nd mask
                    // }
                    // identify tiles
                    TerrainType terrainEnum = GetTerrainTypeFromNoise(a);
                    VegetationType treeEnum = GetTreeTypeFromTerrain(terrainEnum);
                    Cell cell = new Cell(id)
                    {
                        Id = id,
                        Occupied = IsOccupied(terrainEnum, treeEnum),
                        Position = new int2(x, y),
                        TerrainType = terrainEnum,
                        VegetationType = treeEnum
                    };
                    _worldMap.Cells.Add(cell);
                    id ++;
                }
            }
            return _worldMap;
        }

        private bool IsOccupied(TerrainType terrain, VegetationType tree)
        {
            return terrain == TerrainType.Water || tree != VegetationType.None ? true : false;
        }

        private float Noisefunction(float x, float y, Vector2 Origin, float scale) // scale argument added for 2nd mask
        {

            float a = 0, noisesize = scale, opacity = 1;

            for (int octaves = 0; octaves < MapConfig.NoiseOctaves; octaves++)
            {
                float xVal = (x / (noisesize * MapConfig.WorldMapWidth)) + Origin.x;
                float yVal = (y / (noisesize * MapConfig.WorldMapHeight)) - Origin.y;
                float z = noise.snoise(new float2(xVal, yVal));
                a += Mathf.InverseLerp(0, 1, z) / opacity;
                noisesize /= 2f;
                opacity *= 2f;
            }
            return a;
        }

        private VegetationType GetTreeTypeFromTerrain(TerrainType terrainType)
        {
            int randomNumber = UnityEngine.Random.Range(0, 10);
            VegetationType treeType = VegetationType.None;
            if(terrainType == TerrainType.Ground && randomNumber > MapConfig.ChanceGroundTree)
            {
                treeType = VegetationType.PlainTree;
            }
            else if (terrainType == TerrainType.Sand && randomNumber > MapConfig.ChanceBeachTree)
            {
                treeType = VegetationType.BeachTree;
            }
            return treeType;
        }

        private TerrainType GetTerrainTypeFromNoise(float noiseValue)
        {   
            TerrainType type = TerrainType.Water;
            if(noiseValue < -0.2)
            {
                type = TerrainType.Water;
            }
            else if(noiseValue > -0.2 && noiseValue < -0.1)
            {
                type = TerrainType.Sand;
            }
            else if(noiseValue > -0.1 && noiseValue < 0.3)
            {
                type = TerrainType.Ground;
            }
            else if(noiseValue > 0.3 && noiseValue < 0.5)
            {
                type = TerrainType.Mountain;
            }
            else if(noiseValue > 0.5)
            {
                type = TerrainType.Ice;
            }
            return type;
        }
        
        void GenerateCircularGradient()
        {
            int halfWidth = MapConfig.WorldMapWidth / 2;
            int halfHeight = MapConfig.WorldMapHeight / 2;

            _gradientMap = new float[MapConfig.WorldMapWidth, MapConfig.WorldMapHeight];
            for (int x = 0; x < MapConfig.WorldMapWidth; x++)
            {
                for (int y = 0; y < MapConfig.WorldMapHeight; y++)
                {
                    float radius = MapConfig.Radius;
                    float slope = MapConfig.Slope;
                    Vector2 current = new Vector2(x,y);
                    Vector2 center = new Vector2(halfWidth, halfHeight);
                    float distance = Vector2.Distance(current, center);
                    float gradient = 0.0f;
                    if(distance - radius > 0){
                        gradient = slope * Mathf.Pow(distance - radius, 2);
                    }
                    _gradientMap[x, y] = gradient;
                }
            }
        }
    }
}

// Notes:
// 1. Higher noiseScale leads to lesser land feature fragmentation
// 2. Higher octaves lead to more jaggedness
// 3. Higher radius leads to larger island
// 4. Higher slope leads to a lesser micro islands