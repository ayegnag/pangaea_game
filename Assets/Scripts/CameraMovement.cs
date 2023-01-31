using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed = 15f;
    private Tilemap TerrainTilemap;
    float MarginCheck = 10f;
    void Start()
    {
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        Tilemap TerrainTilemap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
        IslandTileSetGenerator GridScript = GameObject.Find("TerrainGrid").GetComponent<IslandTileSetGenerator>();
        Vector3 tilemapPosition = TerrainTilemap.transform.position;
        float posX = tilemapPosition.x + GridScript.TileSizeX / 2;
        float posY = tilemapPosition.y + GridScript.TileSizeY / 2;
        Debug.Log(posX + " " + posY);
        transform.position = new Vector3(posX, posY, -1);
    }

    // Update is called once per frame
    void Update()
    {   
        // float mouseX = Input.GetAxisRaw("Mouse X");
        // float mouseY = Input.GetAxisRaw("Mouse Y");
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        // Debug.Log(mouseX + " : " + mouseY);
        if(mouseX < MarginCheck) {
           transform.position += new Vector3(Time.deltaTime * speed * -1, 0f, 0f);
        }
        else if(mouseX > Screen.width - MarginCheck) {
           transform.position += new Vector3(Time.deltaTime * speed, 0f, 0f);
        }
        if(mouseY < MarginCheck) {
           transform.position += new Vector3(0f,Time.deltaTime * speed * -1, 0f);
        }
        else if(mouseY > Screen.height - MarginCheck) {
           transform.position += new Vector3(0f,Time.deltaTime * speed, 0f);
        }
        // TODO: Add logic to balance double acceleration by both axis in the corners.
        // transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, 0f);
    }
}
