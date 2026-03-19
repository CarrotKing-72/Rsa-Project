using UnityEngine;
using System;


public class Tile : MonoBehaviour
{
    public TileType tileType; // Set in Inspector
    public GridManager gridManager; // Assign in Inspector or Find at runtime

    [Obsolete]
    void Start()
    {
        Vector2Int gridPos = Vector2Int.RoundToInt(transform.position);
        if (!gridManager)
            gridManager = FindObjectOfType<GridManager>();
        gridManager.SetTileType(gridPos, tileType);
    }
}
