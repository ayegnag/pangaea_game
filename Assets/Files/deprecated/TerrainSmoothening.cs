using UnityEngine;

public class TerrainSmoothing : MonoBehaviour
{
    public int smoothingIterations = 5;
    public int wallThresholdSize = 5;
    public int wallHeight = 2;
    public int floorHeight = 0;

    private int[,] terrainMap;
    private int mapSize;

    private void Start()
    {
        mapSize = 10;
        terrainMap = new int[mapSize, mapSize];

        // Initialize terrain map with random values
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                terrainMap[i, j] = Random.Range(floorHeight, wallHeight + 1);
            }
        }

        // Smooth the terrain using the cellular automata method
        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothTerrain();
        }

        // Generate the terrain in Unity
        GenerateTerrain();
    }

    private void SmoothTerrain()
    {
        int[,] newMap = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                int surroundingWalls = GetSurroundingWalls(i, j);

                if (surroundingWalls > wallThresholdSize)
                {
                    newMap[i, j] = wallHeight;
                }
                else
                {
                    newMap[i, j] = floorHeight;
                }
            }
        }

        terrainMap = newMap;
    }

    private int GetSurroundingWalls(int x, int y)
    {
        int wallCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborX = x + i;
                int neighborY = y + j;
                if (neighborX >= 0 && neighborX < mapSize && neighborY >= 0 && neighborY < mapSize)
                {
                    if (terrainMap[neighborX, neighborY] == wallHeight)
                    {
                        wallCount++;
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private void GenerateTerrain()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GameObject terrainBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terrainBlock.transform.position = new Vector3(i, terrainMap[i, j], j);
            }
        }
    }
}
