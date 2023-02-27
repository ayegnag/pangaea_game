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

            Vector2 Org = new Vector2(Mathf.Sqrt(Seed), Mathf.Sqrt(Seed));
            Vector2 center = new Vector2(TileSizeX / 2, TileSizeY / 2);
            Vector2 target = center;
            Vector2 nearCenterPoint = new Vector2(center.x - 40, center.y + 40);
            for (int x = 0, i = 0; x < TileSizeX; x++)
            {
                TileGrid.Add(new List<GameObject>());
                for (int y = 0; y < TileSizeY; y++, i++)
                {
                    float a = Noisefunction(x, y, Org);
                    a = (a * 0.8f) - gradientMap[x, y];
                    //a = (a * 0.8f);
                    target = new Vector2(x, y);
                    //if (a < 0.1 && Vector2.Distance(center, target) < UnityEngine.Random.Range(30, 80))
                    //{
                    //    a = 0.2f;
                    //}
                    //if (a > 0.1 && Vector2.Distance(nearCenterPoint, target) < UnityEngine.Random.Range(30, 50))
                    //{
                    //    a = 0;
                    //}

                    //if (Vector2.Distance(center, target) == 0)
                    //{
                    //    a = 4;
                    //}
                    //if (Vector2.Distance(nearCenterPoint, target) == 0)
                    //{
                    //    a = 4;
                    //}

                    int tileId = GetTileIDFromNoise(a);
                    int treeId = GetRandomTree(tileId);
                    treeMap[x, y] = treeId;
                    terrainMap[x, y] = tileId;
                    CreateTile(tileId, x, y);
                    CreateTreeTile(treeId, x, y);
                }
            }

            int rand1 = 30;
            int rand2 = UnityEngine.Random.Range(30, 80);
            for (int k = 10; k < 510; k++)
            {
                int ycoord = (int)(0 + Math.Sqrt(Math.Pow(250, 2) - Math.Pow((k - 260), 2)));
                int ycoord1 = (int)(ycoord + Noisefunction(k, ycoord, Org) * rand1);
                int ycoord2 = (int)(ycoord + Noisefunction(k, ycoord, Org) * rand2);
                print("circle: " +k + " " + ycoord);
                CreateTile(4, k, ycoord1);
                CreateTile(4, k, -ycoord2);
            }

            //for (int x = 1, i = 0; x < TileSizeX - 1; x++)
            //{
            //    for (int y = 1; y < TileSizeY - 1; y++, i++)
            //    {
            //        target = new Vector2(x, y);
            //        int neighbour1 = terrainMap[x - 1, y] == 0 ? 0 : 1;
            //        int neighbour3 = terrainMap[x + 1, y - 1] == 0 ? 0 : 1;
            //        int neighbour4 = terrainMap[x - 1, y] == 0 ? 0 : 1;
            //        int neighbour5 = terrainMap[x - 1, y + 1] == 0 ? 0 : 1;
            //        int neighbour2 = terrainMap[x + 1, y + 1] == 0 ? 0 : 1;
            //        int neighbour6 = terrainMap[x - 1, y - 1] == 0 ? 0 : 1;
            //        int neighbour7 = terrainMap[x, y + 1] == 0 ? 0 : 1;
            //        int neighbour8 = terrainMap[x, y - 1] == 0 ? 0 : 1;
            //        int sumNeighbours = neighbour1 + neighbour2 + neighbour3 + neighbour4 + neighbour5 + neighbour6 + neighbour7 + neighbour8;

            //        if (terrainMap[x, y] == 0 && sumNeighbours > 6)
            //        {
            //            terrainMap[x, y] = 1;
            //        }
            //        if (terrainMap[x, y] > 0 && sumNeighbours < 3)
            //        {
            //            terrainMap[x, y] = 0;
            //        }
            //        CreateTile((int)terrainMap[x, y], x, y);
            //        CreateTreeTile(GetRandomTree((int)terrainMap[x, y]), x, y);
            //    }
            //}

            //Vector2 center = new Vector2(TileSizeX / 2, TileSizeY / 2);

            //Vector2 exploredTile = new Vector2(0,0);
            //for (int x = 0, i = 0; x < TileSizeX; x++)
            //{
            //    for (int y = 0; y < TileSizeY; y++, i++)
            //    {
            //        Vector2 targetTile = new Vector2(x, y);
            //        if (Vector2.Distance(center, targetTile) > 10 && terrainMap[(int)targetTile.x, (int)targetTile.y] > 0.1)
            //        {
            //            Vector2 direction = (center - targetTile).normalized;
            //            bool settled = false;
            //            int k = 1;
            //            while (!settled)
            //            {
            //                k++;
            //                exploredTile = targetTile + direction * k;
            //                settled = terrainMap[(int)exploredTile.x, (int)exploredTile.y] > 0.1 || Vector2.Distance(center, exploredTile) < 5;
            //            }
            //            k--;
            //            exploredTile = targetTile + direction * k;
            //            terrainMap[(int)exploredTile.x, (int)exploredTile.y] = terrainMap[(int)targetTile.x, (int)targetTile.y];
            //            terrainMap[x, y] = 0;
            //            CreateTile(0, x, y);
            //            CreateTile((int)terrainMap[(int)exploredTile.x, (int)exploredTile.y], (int)exploredTile.x, (int)exploredTile.y);
            //        }
            //    }45918884
            //}


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
            //if(noiseValue < 0.1)
            //{
            //    id = 0; // water
            //}
            //else if (noiseValue > 0.1 && noiseValue < 0.3)
            //{
            //    id = 1; // sand
            //}
            //else if (noiseValue > 0.3 && noiseValue < 0.5)
            //{
            //    id = 2; // grassland
            //}
            //else if (noiseValue > 0.5 && noiseValue < 0.8)
            //{
            //    id = 3; // mountain
            //}
            //else if (noiseValue > 0.8)
            //{
            //    id = 4; // ice
            //}


            ////////////////////////////////////////
            ///

            if (noiseValue < -0.2)
            {
                id = 0; // water
            }
            else if (noiseValue > -0.2 && noiseValue < -0.1)
            {
                id = 1; // sand
            }
            else if (noiseValue > -0.1 && noiseValue < 0.1)
            {
                id = 2; // grassland
            }
            else if (noiseValue > 0.1 && noiseValue < 0.4)
            {
                id = 3; // mountain
            }
            else if (noiseValue > 0.4)
            {
                id = 4; // ice
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