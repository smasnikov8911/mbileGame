using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapCamera : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap, который нужно подстроить
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        AdjustTilemap();
    }

    void Update()
    {
        AdjustTilemap();
    }

    void AdjustTilemap()
    {
        float cameraHeight = _mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * _mainCamera.aspect;

        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int tileSize = bounds.size;

        // Увеличиваем размер Tilemap, если он меньше, чем камера
        if (tileSize.x < cameraWidth || tileSize.y < cameraHeight)
        {
            for (int x = bounds.xMin; x < bounds.xMin + cameraWidth; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMin + cameraHeight; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tilemap.GetTile(Vector3Int.zero));
                }
            }
        }

        // Центрируем Tilemap относительно камеры
        tilemap.transform.position = new Vector3(-cameraWidth / 2, -cameraHeight / 2, 0);
    }
}
