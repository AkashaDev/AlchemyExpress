using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronGrid : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize = 1f;

    public bool[,] gridOccupied;

    private void Awake()
    {
        gridOccupied = new bool[width, height];
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos) 
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    public Vector2Int GetGridPosition(Vector3 worldPos) 
    {
        return new Vector2Int
            (
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.y / cellSize)
            );
    }

    public bool CanPlaceIngridient(IngredientObject ingredient, Vector2Int anchorPos) 
    {
        var shape = ingredient.data.shapeVariants[ingredient.variantIndex];
        foreach (var offset in shape.tilePositions) 
        {
             Vector2Int gridPos = anchorPos + offset;
            if(gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height)
                return false;
            if (gridOccupied[gridPos.x, gridPos.y])
                return false;
        }
        return true;
    }

    public void PlaceIngridient(IngredientObject ingredient, Vector2Int anchorPos) 
    {
        var shape = ingredient.data.shapeVariants[ingredient.variantIndex];
        foreach (var offset in shape.tilePositions) 
        {
            Vector2Int gridPos = anchorPos + offset;
            gridOccupied[gridPos.x, gridPos.y] = true;
        }
        ingredient.transform.position = GetWorldPosition(anchorPos);
    }
}
