using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Text;
using UnityEngine.Tilemaps;
namespace KOI
{
    public class IslandTileSetGenerator : MonoBehaviour
    {
        public int TileSizeX, TileSizeY;
        public float NoiseScale, IslandSize;
        public float constX, constY;
        [Range(1, 20)] 
        public int NoiseOctaves;
        [Range(0, 99999999)] 
        public int Seed;

        Dictionary<int, Tile> tileset;
        Dictionary<int, Tile> treeTileset;
        public Tile prefabWater;
        public Tile prefabSand;
        public Tile prefabGrass;
        public Tile prefabStone;
        public Tile prefabIce;
        public Tile prefabTreeOne;
        public Tile prefabTreeTwo;
        
        [Range(1, 10)] 
        public int chanceGroundTree;
        [Range(1, 10)] 
        public int chanceBeachTree;
        
        // Privates
        private int TileSize = 1;
        private Tilemap terrainTileMap;
        private Tilemap treesTileMap;
        private float[,] gradientMap;


        List<List<GameObject>> TileGrid = new List<List<GameObject>>();

        private void Awake()
        {
            treesTileMap = GetComponentsInChildren<Tilemap>()[0];
            terrainTileMap = GetComponentsInChildren<Tilemap>()[1];
            terrainTileMap.ClearAllTiles();
            treesTileMap.ClearAllTiles();
        }

        private void Start()
        {
            GenerateRectangularGradient();
            CreateTileSet();
            RenderGeneratedMap();
        }

        private void CreateTileSet()
        {
            tileset = new Dictionary<int, Tile>();
            tileset.Add(0, prefabWater);
            tileset.Add(1, prefabSand);
            tileset.Add(2, prefabGrass);
            tileset.Add(3, prefabStone);
            tileset.Add(4, prefabIce);

            treeTileset = new Dictionary<int, Tile>();
            treeTileset.Add(1, prefabTreeOne);
            treeTileset.Add(2, prefabTreeTwo);
        }

        void RenderGeneratedMap()
        {
            float[,] terrainMap = new float[TileSizeX, TileSizeY];
            float[,] treeMap = new float[TileSizeX, TileSizeY];
            // float[,] aMap = new float[TileSizeX, TileSizeY];

            Vector2 Org = new Vector2(Mathf.Sqrt(Seed), Mathf.Sqrt(Seed));

            for (int x = 0, i = 0; x < TileSizeX; x++)
            {
                TileGrid.Add(new List<GameObject>());
                for (int y = 0; y < TileSizeY; y++, i++)
                {
                    float a = Noisefunction(x, y, Org);
                    a = (a * 0.8f) - gradientMap[x, y]; // 0.9 => 0.72 - 1 = -0.29
                    // aMap[x, y] = a;
                    // a = (a + (1-distanceSquared(x, y)));
                    // int tileId = 0;
                    // if(distanceSquared(x, y) > 0)
                    // if(gradientMap[x, y] < 0.5)
                    // {
                    // Debug.Log(x + " " + y + " " + distanceSquared(x, y));
                    // }
                    int tileId = GetTileIDFromNoise(a);
                    int treeId = GetRandomTree(tileId);
                    treeMap[x, y] = treeId;
                    terrainMap[x, y] = tileId;
                    CreateTile(tileId, x, y);
                    CreateTreeTile(treeId, x, y);
                }
            }
            
            // print("Map: ");
            // MapLogger(aMap);
            // MapLogger(terrainMap);
        }

        private float Noisefunction(float x, float y, Vector2 Origin)
        {
            float a = 0, noisesize = NoiseScale, opacity = 1;

            for (int octaves = 0; octaves < NoiseOctaves; octaves++)
            {
                float xVal = (x / (noisesize * TileSizeX)) + Origin.x;
                float yVal = (y / (noisesize * TileSizeY)) - Origin.y;
                float z = noise.snoise(new float2(xVal, yVal));
                // float z = noise.pnoise(new float2(xVal, yVal), 0);
                a += Mathf.InverseLerp(0, 1, z) / opacity;
                noisesize /= 2f;
                opacity *= 2f;
            }
            // a -= gradientMap[(int)x, (int)y];
            return a;
            // return a -= FallOffMap(x, y, TileSizeX, TileSizeY, IslandSize);
        }

        private int GetRandomTree(int terrainType)
        {
            int randomNumber = UnityEngine.Random.Range(0, 10);
            int treeType = 0;
            if(terrainType == 2 && randomNumber > chanceGroundTree)
            {
                treeType = 1;
            }
            else if (terrainType == 1 && randomNumber > chanceBeachTree)
            {
                treeType = 2;
            }
            return treeType;
        }

        private int GetTileIDFromNoise(float noiseValue)
        {   
            // float scaledNoise = noiseValue * TileCount;
            // int id = Mathf.FloorToInt(scaledNoise);
            int id = 0;
            if(noiseValue < 0.1)
            {
                id = 0;
            }
            else if(noiseValue > 0.1 && noiseValue < 0.3)
            {
                id = 1;
            }
            else if(noiseValue > 0.3 && noiseValue < 0.5)
            {
                id = 2;
            }
            else if(noiseValue > 0.5 && noiseValue < 0.8)
            {
                id = 3;
            }
            else if(noiseValue > 0.8)
            {
                id = 4;
            }
            return id;
        }

        private void CreateTile(int tileId, int x, int y)
        {
            Tile tilePrefab = tileset[tileId];
            Vector3Int localPosition = new Vector3Int(x * TileSize, y * TileSize, 0);
            terrainTileMap.SetTile(localPosition, tilePrefab);
        }
        private void CreateTreeTile(int treeId, int x, int y)
        {
            if (treeId > 0)
            {
                // Debug.Log(treeId);
                Tile treePrefab = treeTileset[treeId];
                Vector3Int localPosition = new Vector3Int(x * TileSize, y * TileSize, 0);
                treesTileMap.SetTile(localPosition, treePrefab);
            }
        }


        void GenerateRectangularGradient()
        {
            // int N = 4;
            gradientMap = new float[TileSizeX, TileSizeY];
            for (int x = 0; x < TileSizeX; x++)
            {
                for (int y = 0; y < TileSizeY; y++)
                {
                    // float xNormalized = (float)x / TileSizeX;
                    // float yNormalized = (float)y / TileSizeY;

                    // float gradient = Mathf.Lerp(1, 0, Mathf.Max(xNormalized, yNormalized));
                    float xValue = Math.Abs(x * 2f - TileSizeX) / TileSizeX;
                    float yValue = Math.Abs(y * 2f - TileSizeY) / TileSizeY;
                    float gradient = Math.Max(xValue, yValue);
                    // int index = Mathf.FloorToInt( gradient * N );
                    // index = (index == N) ? N - 1 : index ;
                    // gradientMap[x, y] = index;
                    gradientMap[x, y] = gradient;
                }
            }
        }

        float distanceSquared(int x, int y)
        {
            float dx = 2 * x / TileSizeX - 1;
            float dy = 2 * y / TileSizeY - 1;
            // at this point 0 <= dx <= 1 and 0 <= dy <= 1
            // float sqrd = dx*dx + dy*dy;
            float sqrd = 1 - (1-dx*dx) * (1-dy*dy);
    
            // Debug.Log(x + " " + y + " " + sqrd.ToString());
            return sqrd;
        }

        // private float FallOffMap(float x, float y, int sizeX, int sizeY, float islandSize)
        // {
        //     float gradient = 1;

        //     gradient /= (x * y) / (sizeX * sizeY) * (1 - (x / sizeX)) * (1 - (y / sizeY));
        //     // gradient -= 16;
        //     // gradient /= islandSize;
        //     // Debug.Log(gradient);
        //     return gradient;
        // }

        // Utility tool just for printing metrix.
        private void MapLogger(float[,] map){
            StringBuilder sb = new StringBuilder();
            // print("X:" + map.GetLength(0) + "Y:" + map.GetLength(1));
            for(int i=0; i < map.GetLength(0); i++)
            {
                for(int j=0; j < map.GetLength(1); j++)
                {
                    sb.Append(map [i,j]);
                    sb.Append(' ');				   
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }
    }
}