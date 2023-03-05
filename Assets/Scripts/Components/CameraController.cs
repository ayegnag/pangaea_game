using KOI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Tilemap TerrainTilemap;

    private float cameraSpeed = GameConfig.CameraSpeed;
    private float edgeSensitivity = GameConfig.EdgeSensitivity;
    private float zoomSpeed = GameConfig.ZoomSpeed;
    private float minZoom = GameConfig.MinZoom;
    private float maxZoom = GameConfig.MaxZoom;

    private Vector3 moveDirection;
    private float currentZoom;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float screenWidth = Screen.width;
    private float screenHeight = Screen.height;
    private BoundsInt bounds;
    private bool initialized = false;
    private void Start()
    {}

    private void LateUpdate()
    {
        if (!initialized)
        {
            bounds = TerrainTilemap.cellBounds;
            bottomLeftLimit = TerrainTilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
            topRightLimit = TerrainTilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));
            transform.position = new Vector3(topRightLimit.x/2, topRightLimit.y/2, -1);
            initialized = true;
        }
    }

    private void Update()
    {
        Camera camera = Camera.main;
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        moveDirection = Vector3.zero;
        if (mouseX < edgeSensitivity)
        {
            moveDirection.x = -1;
        }
        else if (mouseX > screenWidth - edgeSensitivity)
        {
            moveDirection.x = 1;
        }

        if (mouseY < edgeSensitivity)
        {
            moveDirection.y = -1;
        }
        else if (mouseY > screenHeight - edgeSensitivity)
        {
            moveDirection.y = 1;
        }
        
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Camera.main.orthographicSize = currentZoom;

        float cameraWidth = camera.aspect * camera.orthographicSize;
        float cameraHeight = camera.orthographicSize;

        moveDirection = GetSafeNormalization(moveDirection);
        Vector3 newPos = transform.position + moveDirection * cameraSpeed * Time.deltaTime * (0.2f) * currentZoom;
        newPos.x = Mathf.Clamp(newPos.x, bottomLeftLimit.x + cameraWidth, topRightLimit.x - cameraWidth);
        newPos.y = Mathf.Clamp(newPos.y, bottomLeftLimit.y + cameraHeight, topRightLimit.y - cameraHeight);
        transform.position = newPos;

    }
    
    private static Vector3 GetSafeNormalization(Vector3 vector) {
      if (vector == Vector3.zero) {
         return Vector3.zero;
      }
      vector.Normalize();
      return vector;
    }
}
