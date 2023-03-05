using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float mouseSpeed = 35f;
    public float scrollSpeed = 3f;
    public float cameraMinSize = 15f;
    public float cameraMaxSize = 35f;
    public float edgeSensitivity = 10f;
    public Tilemap TerrainTilemap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    float mapSizeX;
    float mapSizeY;
    float halfHeight;
    float halfWidth;
    private bool initialized = false;
    private BoundsInt bounds;
    void Start()
    {
        // Camera camera = Camera.main;
        // halfHeight = camera.orthographicSize;
        // halfWidth = camera.aspect * halfHeight;
        // Tilemap TerrainTilemap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();


        
        bounds = TerrainTilemap.cellBounds;
        // bottomLeftLimit = TerrainTilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
        // topRightLimit = TerrainTilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));
        // Debug.Log(bounds);
        // Debug.Log(bottomLeftLimit + " " + topRightLimit);
        // IslandTileSetGenerator GridScript = GameObject.Find("TerrainGrid").GetComponent<IslandTileSetGenerator>();
        // Vector3 tilemapPosition = TerrainTilemap.transform.position;
        // float posX = tilemapPosition.x + GridScript.TileSizeX / 2;
        // float posY = tilemapPosition.y + GridScript.TileSizeY / 2;
        // mapSizeX = tilemapPosition.x + GridScript.TileSizeX;
        // mapSizeY = tilemapPosition.y + GridScript.TileSizeY;
        // Debug.Log(posX + " " + posY);
        // Debug.Log(TerrainTilemap);
        // transform.position = new Vector3(posX, posY, -1);
    }

    // Update is called once per frame
    void LateUpdate()
    {        
        if (!initialized) {
            bounds = TerrainTilemap.cellBounds;
            bottomLeftLimit = TerrainTilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
            topRightLimit = TerrainTilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));
            // Debug.Log(bounds);
            // Debug.Log(bottomLeftLimit + " " + topRightLimit);
            transform.position = new Vector3(topRightLimit.x/2, topRightLimit.y/2, -1);
            initialized = true;
        }
    }

    void Update() {
        Camera camera = Camera.main;

        Vector3 direction = Vector3.zero;
        direction = GetCameraMovement();
        // if(IfWithinBounds(direction)) {
        //     transform.position += new Vector3(direction.x, direction.y, 0f);
        // }
        if(IfWithinBounds(direction)) {
        HandleCameraZoom();
        // transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * MouseSpeed, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MouseSpeed, 0f);
        camera.transform.position = new Vector3(
            Mathf.Clamp(camera.transform.position.x + direction.x, bottomLeftLimit.x + camera.orthographicSize, topRightLimit.x - camera.orthographicSize),
            Mathf.Clamp(camera.transform.position.y + direction.y, bottomLeftLimit.y + camera.orthographicSize, topRightLimit.y - camera.orthographicSize),
            0f
        );
        }
    }
private Vector3 GetCameraMovement() {
// float mouseX = Input.GetAxisRaw("Mouse X");
        // float mouseY = Input.GetAxisRaw("Mouse Y");
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        // Debug.Log(mouseX + " : " + mouseY);
        Vector3 direction = Vector3.zero;

        if(mouseX < edgeSensitivity) {
        //   transform.position += new Vector3(Time.deltaTime * speed * -1, 0f, 0f);
        direction.x -= 1; 
        }
        else if(mouseX > Screen.width - edgeSensitivity) {
        //   transform.position += new Vector3(Time.deltaTime * speed, 0f, 0f);
            direction.x += 1; 
        }
        if(mouseY < edgeSensitivity) {
        //   transform.position += new Vector3(0f,Time.deltaTime * speed * -1, 0f);
            direction.y -= 1; 
        }
        else if(mouseY > Screen.height - edgeSensitivity) {
        //   transform.position += new Vector3(0f,Time.deltaTime * speed, 0f);
            direction.y += 1; 
        }
        direction = GetSafeNormalization(direction);
        direction = direction * mouseSpeed * Time.deltaTime;
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
            if(camera.orthographicSize < cameraMinSize) {
                camera.orthographicSize = cameraMinSize;    
            }
            else if(camera.orthographicSize > cameraMinSize) {
                camera.orthographicSize -= scrollSpeed;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if(camera.orthographicSize > cameraMaxSize) {
                camera.orthographicSize = cameraMaxSize;    
            }
            else if(camera.orthographicSize < cameraMaxSize) {
                camera.orthographicSize += scrollSpeed;
            }
        }
        // Update Camera size
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;
    }
}
