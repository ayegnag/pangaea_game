using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Text;
using UnityEngine.Tilemaps;

public class IslandTileMapGenerator : MonoBehaviour
{
    public int TileSizeX, TileSizeY;
    public float NoiseScale, IslandSize;
    [Range(1, 20)] 
    public int NoiseOctaves;
    [Range(0, 99999999)] 
    public int Seed;

    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;
    public GameObject prefabWater;
    public GameObject prefabSand;
    public GameObject prefabGrass;
    public GameObject prefabStone;
    public GameObject prefabIce;

    // Privates

    private Color[] colour;
    private Texture2D texture;
    private int TileCount = 3;
    private float TileSize = 0.16f;

    List<List<GameObject>> TileGrid = new List<List<GameObject>>();
    // public Gradient ColorGradient;

    private void Start()
    {
        CreateTileSet();
        CreateTileGroups();
        RenderGeneratedMap();
    }

    private void CreateTileSet()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefabWater);
        tileset.Add(1, prefabSand);
        tileset.Add(2, prefabGrass);
        tileset.Add(3, prefabStone);
        tileset.Add(4, prefabIce);
    }
    
    private void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> prefabPair in tileset)
        {
            GameObject tileGroup = new GameObject(prefabPair.Value.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = new Vector2(0, 0);
            tileGroups.Add(prefabPair.Key, tileGroup);
        }
    }

    void RenderGeneratedMap()
    {
        float[,] terrainMap = new float[TileSizeX, TileSizeY];
        texture = new Texture2D(TileSizeX, TileSizeY);
        colour = new Color[texture.height * texture.width];

        Vector2 Org = new Vector2(Mathf.Sqrt(Seed), Mathf.Sqrt(Seed));

        for (int x = 0, i = 0; x < TileSizeX; x++){
            TileGrid.Add(new List<GameObject>());
            for (int y = 0; y < TileSizeY; y++, i++){
                float a = Noisefunction(x, y, Org);
                int tileId = GetTileIDFromNoise(a);
                terrainMap[x, y] = tileId;
                // print( x +" " + y +" "+ tileId);
                CreateTile(tileId, x, y);
                // colour[i] = ColorGradient.Evaluate(Noisefunction((float)x, (float)y, Org));
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
        GameObject tilePrefab = tileset[tileId];
        GameObject tileGroup = tileGroups[tileId];
        GameObject tile = Instantiate(tilePrefab, tileGroup.transform);
        tile.isStatic = true;

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector2(x * TileSize, y * TileSize);
        TileGrid[x].Add(tile);
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