using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private float MouseSpeed = 35f;
    private float ScrollSpeed = 3f;
    private float CameraMinSize = 15f;
    private float CameraMaxSize = 35f;
    private Tilemap TerrainTilemap;
    float MarginCheck = 10f;
    float mapSizeX;
    float mapSizeY;
    float halfHeight;
    float halfWidth;
    void Start()
    {
        Camera camera = Camera.main;
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;
        Tilemap TerrainTilemap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
        IslandTileSetGenerator GridScript = GameObject.Find("TerrainGrid").GetComponent<IslandTileSetGenerator>();
        Vector3 tilemapPosition = TerrainTilemap.transform.position;
        float posX = tilemapPosition.x + GridScript.TileSizeX / 2;
        float posY = tilemapPosition.y + GridScript.TileSizeY / 2;
        mapSizeX = tilemapPosition.x + GridScript.TileSizeX;
        mapSizeY = tilemapPosition.y + GridScript.TileSizeY;
        Debug.Log(posX + " " + posY);
        Debug.Log(TerrainTilemap);
        transform.position = new Vector3(posX, posY, -1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        direction = GetCameraMovement();
        if(IfWithinBounds(direction)) {
            transform.position += new Vector3(direction.x, direction.y, 0f);
        }
        HandleCameraZoom();
        // transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, 0f);
    }
private Vector3 GetCameraMovement() {
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
        direction = direction * MouseSpeed * Time.deltaTime;
        return direction;
}
    private bool IfWithinBounds(Vector3 delta) {
        if(transform.position.x + delta.x < halfWidth || transform.position.x + delta.x > mapSizeX - halfWidth
        || transform.position.y + delta.y < halfHeight || transform.position.y + delta.y > mapSizeY - halfHeight) {
           return false;
        }
        return true;
    }

    private static Vector3 GetSafeNormalization(Vector3 vector) {
      if (vector == Vector3.zero) {
         return Vector3.zero;
      }
      vector.Normalize();
      return vector;
    }

    private void HandleCameraZoom() {
        Camera camera = Camera.main;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if(camera.orthographicSize < CameraMinSize) {
                camera.orthographicSize = CameraMinSize;    
            }
            else if(camera.orthographicSize > CameraMinSize) {
                camera.orthographicSize -= ScrollSpeed;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if(camera.orthographicSize > CameraMaxSize) {
                camera.orthographicSize = CameraMaxSize;    
            }
            else if(camera.orthographicSize < CameraMaxSize) {
                camera.orthographicSize += ScrollSpeed;
            }
        }
        // Update Camera size
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;
    }
}
