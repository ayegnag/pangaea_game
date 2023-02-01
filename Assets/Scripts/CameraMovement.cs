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
        Vector3 direction = Vector3.zero;

        if(mouseX < MarginCheck) {
         //   transform.position += new Vector3(Time.deltaTime * speed * -1, 0f, 0f);
         direction.x -= 1; 
        }
        else if(mouseX > Screen.width - MarginCheck) {
         //   transform.position += new Vector3(Time.deltaTime * speed, 0f, 0f);
            direction.x += 1; 
        }
        if(mouseY < MarginCheck) {
         //   transform.position += new Vector3(0f,Time.deltaTime * speed * -1, 0f);
            direction.y -= 1; 
        }
        else if(mouseY > Screen.height - MarginCheck) {
         //   transform.position += new Vector3(0f,Time.deltaTime * speed, 0f);
            direction.y += 1; 
        }
        direction = GetSafeNormalization(direction);
        direction = direction * speed * Time.deltaTime;
      //   Debug.Log(direction);
        transform.position += new Vector3(direction.x, direction.y, 0f);
        // transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, 0f);
    }

    private static Vector3 GetSafeNormalization(Vector3 vector) {
      if (vector == Vector3.zero) {
         return Vector3.zero;
      }
      vector.Normalize();
      return vector;
    }
}
