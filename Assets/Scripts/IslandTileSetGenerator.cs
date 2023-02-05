using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Text;
using UnityEngine.Tilemaps;

public class IslandTileSetGenerator : MonoBehaviour
{
    public int TileSizeX, TileSizeY;
    public float NoiseScale, IslandSize;
    [Range(1, 20)] 
    public int NoiseOctaves;
    [Range(0, 99999999)] 
    public int Seed;

    Dictionary<int, Tile> tileset;
    public Tile prefabWater;
    public Tile prefabSand;
    public Tile prefabGrass;
    public Tile prefabStone;
    public Tile prefabIce;

    // Privates
    private int TileCount = 3;
    private int TileSize = 1;
    private Tilemap terrainTileMap;

    List<List<GameObject>> TileGrid = new List<List<GameObject>>();

    private void Start()
    {
        terrainTileMap = GetComponentsInChildren<Tilemap>()[0];
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
    }

    void RenderGeneratedMap()
    {
        float[,] terrainMap = new float[TileSizeX, TileSizeY];

        Vector2 Org = new Vector2(Mathf.Sqrt(Seed), Mathf.Sqrt(Seed));

        for (int x = 0, i = 0; x < TileSizeX; x++){
            TileGrid.Add(new List<GameObject>());
            for (int y = 0; y < TileSizeY; y++, i++){
                float a = Noisefunction(x, y, Org);
                int tileId = GetTileIDFromNoise(a);
                terrainMap[x, y] = tileId;
                // print( x +" " + y +" "+ tileId);
                CreateTile(tileId, x, y);
            }
        }
        
        // print("Map: ");
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
            a += Mathf.InverseLerp(0, 1, z) / opacity;
            noisesize /= 2f;
            opacity *= 2f;
        }
        return a;
        // return a -= FallOffMap(x, y, TileSizeX, TileSizeY, IslandSize);
    }

    private int GetTileIDFromNoise(float noiseValue)
    {   
        float scaledNoise = noiseValue * TileCount;
        return Mathf.FloorToInt(scaledNoise);
    }

    private void CreateTile(int tileId, int x, int y)
    {
        Tile tilePrefab = tileset[tileId];
        Vector3Int localPosition = new Vector3Int(x * TileSize, y * TileSize, 0);
        terrainTileMap.SetTile(localPosition, tilePrefab);
    }

    private float FallOffMap(float x, float y, int sizeX, int sizeY, float islandSize)
    {
        float gradient = 1;

        gradient /= (x * y) / (sizeX * sizeY) * (1 - (x / sizeX)) * (1 - (y / sizeY));
        // gradient -= 16;
        // gradient /= islandSize;
        Debug.Log(gradient);
        return gradient;
    }

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