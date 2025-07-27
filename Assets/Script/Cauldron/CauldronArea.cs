using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronArea : MonoBehaviour
{
    public BoxCollider2D cauldronCollider;
    public float gridSize = 1f;
    public int gridWidth = 5;
    public int gridHeight = 5;

    private bool[,] gridOccupied;
    private Vector3 gridOrigin;

    private void Awake()
    {
        if (cauldronCollider == null)
            cauldronCollider = GetComponent<BoxCollider2D>();

        gridOccupied = new bool[gridWidth, gridHeight];

        gridOrigin = cauldronCollider.bounds.min;
    }

    private void OnDrawGizmos()
    {
        if (cauldronCollider == null) return;

        Gizmos.color = Color.gray;

        Vector3 origin = cauldronCollider.bounds.min;

        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = origin + new Vector3(x * gridSize, 0, 0);
            Vector3 end = start + new Vector3(0, gridHeight * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = origin + new Vector3(0, y * gridSize, 0);
            Vector3 end = start + new Vector3(gridWidth * gridSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    public bool IsInside(Vector3 worldPos)
    {
        return cauldronCollider.OverlapPoint(worldPos);
    }

    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;
        int x = Mathf.FloorToInt(local.x / gridSize);
        int y = Mathf.FloorToInt(local.y / gridSize);

        return gridOrigin + new Vector3(x * gridSize, y * gridSize, 0);
    }

    public bool IsGridOccupied(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;
        int x = Mathf.FloorToInt(local.x / gridSize);
        int y = Mathf.FloorToInt(local.y / gridSize);

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return true;

        return gridOccupied[x, y];
    }

    public void MarkGrid(Vector3 worldPos, bool occupied)
    {
        Vector3 local = worldPos - gridOrigin;
        int x = Mathf.FloorToInt(local.x / gridSize);
        int y = Mathf.FloorToInt(local.y / gridSize);

        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            gridOccupied[x, y] = occupied;
        }
    }
}
