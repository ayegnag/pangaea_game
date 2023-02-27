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
            GenerateRectangularGradient();
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
                    float a = Noisefunction(x, y, Org);
                    // clear out the edges to get a central island
                    a = (a * 0.8f) - _gradientMap[x, y]; // 0.9 => 0.72 - 1 = -0.29
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

        private float Noisefunction(float x, float y, Vector2 Origin)
        {

            float a = 0, noisesize = MapConfig.NoiseScale, opacity = 1;

            for (int octaves = 0; octaves < MapConfig.NoiseOctaves; octaves++)
            {
                float xVal = (x / (noisesize * MapConfig.WorldMapWidth)) + Origin.x;
                float yVal = (y / (noisesize * MapConfig.WorldMapHeight)) - Origin.y;
                float z = noise.snoise(new float2(xVal, yVal));
                // float z = noise.pnoise(new float2(xVal, yVal), 0);
                a += Mathf.InverseLerp(0, 1, z) / opacity;
                noisesize /= 2f;
                opacity *= 2f;
            }
            // a -= gradientMap[(int)x, (int)y];
            return a;
            // return a -= FallOffMap(x, y, MapConfig.WorldMapWidth, MapConfig.WorldMapHeight, IslandSize);
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
            else if(noiseValue > -0.1 && noiseValue < 0.1)
            {
                type = TerrainType.Ground;
            }
            else if(noiseValue > 0.1 && noiseValue < 0.4)
            {
                type = TerrainType.Mountain;
            }
            else if(noiseValue > 0.4)
            {
                type = TerrainType.Ice;
            }
            return type;
        }
        
        void GenerateRectangularGradient()
        {
            int halfWidth = MapConfig.WorldMapWidth / 2;
            int halfHeight = MapConfig.WorldMapHeight / 2;

            _gradientMap = new float[MapConfig.WorldMapWidth, MapConfig.WorldMapHeight];
            for (int x = 0; x < MapConfig.WorldMapWidth; x++)
            {
                for (int y = 0; y < MapConfig.WorldMapHeight; y++)
                {
                    // float xValue = Math.Abs(x * 2f - MapConfig.WorldMapWidth) / MapConfig.WorldMapWidth;
                    // float yValue = Math.Abs(y * 2f - MapConfig.WorldMapHeight) / MapConfig.WorldMapHeight;
                    // float gradient = Math.Max(xValue, yValue);
                    // _gradientMap[x, y] = gradient;
                    float radius = 3.0f;
                    float slope = 0.0003f;
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
// 1. Higher noiseScale leads to more land feature fragmentation
// 2. Higher octaves lead to more jaggedness
// 3. Higher radius leads to larger island
// 4. Higher slope leads to a lesser micro islands