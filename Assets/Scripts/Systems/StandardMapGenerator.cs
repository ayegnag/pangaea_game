using System;
using Unity.Mathematics;
using UnityEngine;

namespace KOI
{
    public class StandardMapGenerator: WorldGenerator
    {
        private float[,] _gradientMap;
        private WorldMap _worldMap;
        private MathUtil _mathUtil = new MathUtil();

        public void Initialize(WorldMap worldMap)
        {
            GenerateRectangularGradient();
            _worldMap = worldMap;
        }

        public WorldMap GenerateMap()
        {
            Vector2 Org = new Vector2(Mathf.Sqrt(MapConfig.Seed), Mathf.Sqrt(MapConfig.Seed));
            int id = 0;

            for (int x = 0, i = 0; x < MapConfig.WorldMapWidth; x++)
            {
                for (int y = 0; y < MapConfig.WorldMapHeight; y++, i++)
                {
                    // get original noise map
                    float a = Noisefunction(x, y, Org);
                    // clear out the edges to get a central island
                    // a = (a * 0.8f) - _gradientMap[x, y]; // 0.9 => 0.72 - 1 = -0.29
                    a = a - _gradientMap[x, y]; // 0.9 => 0.72 - 1 = -0.29
                    a = _mathUtil.LinearConverstion(a, 0f, 1.4f, 0f, 1f);
                    // identify tiles
                    TerrainType terrainEnum = GetTerrainTypeFromNoise(a);
                    VegetationType treeEnum = GetTreeTypeFromTerrain(terrainEnum);
                    Cell cell = new Cell(id)
                    {
                        Id = id,
                        Solid = false,
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
        
        private float Noisefunction(float x, float y, Vector2 Origin)
        {
            float a = 0, noiseScale = MapConfig.NoiseScale, opacity = 1;

            for (int octaves = 0; octaves < MapConfig.NoiseOctaves; octaves++)
            {
                // float xVal = (x / (noiseScale * MapConfig.WorldMapWidth)) + Origin.x;
                // float yVal = (y / (noiseScale * MapConfig.WorldMapHeight)) - Origin.y;
                float xVal = (x * noiseScale) + Origin.x;
                float yVal = (y * noiseScale) + Origin.y;
                // float z = noise.pnoise(new float2(xVal, yVal));
                float z = Mathf.PerlinNoise(xVal, yVal);
                a += Mathf.InverseLerp(0, 1, z) / opacity;
                noiseScale /= 2f;
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
            if(noiseValue < 0.1)
            {
                type = TerrainType.Water;
            }
            else if(noiseValue > 0.1 && noiseValue < 0.3)
            {
                type = TerrainType.Sand;
            }
            else if(noiseValue > 0.3 && noiseValue < 0.5)
            {
                type = TerrainType.Ground;
            }
            else if(noiseValue > 0.5 && noiseValue < 0.8)
            {
                type = TerrainType.Mountain;
            }
            else if(noiseValue > 0.8)
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
                    float colorValue;

                    x = x > halfWidth ? MapConfig.WorldMapWidth - x : x;
                    y = y > halfHeight ? MapConfig.WorldMapHeight - y : y;

                    int smaller = x < y ? x : y;
                    colorValue = smaller / (float)halfWidth;

                    colorValue = 1 - colorValue;
                    colorValue *= colorValue * colorValue;
                    float gradient = Math.Max(colorValue, colorValue);
                    _gradientMap[x, y] = gradient;
                }
            }
        }

        // public Color[][] GenerateSquareGradient()
        // {
        //     int width = 500;
        //     int height = 500;
        //     int halfWidth = width / 2;
        //     int halfHeight = height / 2;

        //     Color[][] gradient = new Color[width][];

        //     for (int i = 0; i < width; i++)
        //     {
        //         gradient[i] = new Color[height];

        //         for (int j = 0; j < height; j++)
        //         {
        //             int x = i;
        //             int y = j;

        //             float colorValue;

        //             x = x > halfWidth ? width - x : x;
        //             y = y > halfHeight ? height - y : y;

        //             int smaller = x < y ? x : y;
        //             colorValue = smaller / (float)halfWidth;

        //             colorValue = 1 - colorValue;
        //             colorValue *= colorValue * colorValue;
        //             gradient[i][j] = new Color(colorValue, colorValue, colorValue);
        //         }
        //     }

        //     return gradient;
        // }
    }
}