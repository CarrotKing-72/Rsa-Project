using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Concrete,
    Tree,
    EndTile
}

public class GridManager : MonoBehaviour
{
    
    private Dictionary<Vector2Int, TileType> tiles = new Dictionary<Vector2Int, TileType>();

    [Header("Tile Effects")]
    public float concreteHeatRate = 10f;
    public float treeCoolRate = -2f;    

    public TileType GetTileTypeAtPosition(Vector2Int gridPos)
    {
        if (tiles.TryGetValue(gridPos, out TileType type))
            return type;
        return TileType.Concrete; 
    }

    public float GetTemperatureEffect(TileType type)
    {
        switch (type)
        {
            case TileType.Concrete: return concreteHeatRate;
            case TileType.Tree: return treeCoolRate;
            case TileType.EndTile: return treeCoolRate;
            default: return 0f;
        }
    }

    
    public void SetTileType(Vector2Int gridPos, TileType type)
    {
        tiles[gridPos] = type;
    }
}
