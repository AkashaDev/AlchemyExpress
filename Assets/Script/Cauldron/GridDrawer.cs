using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    public float gridSize = 1f;
    public Color lineColor = Color.black;

    private void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        GameObject parent = new GameObject("GridLines");
        parent.transform.SetParent(transform);

        Vector3 origin = GetComponent<BoxCollider2D>().bounds.min;

        for (int x = 0; x <= width; x++)
        {
            CreateLine(origin + new Vector3(x * gridSize, 0, 0),
                       origin + new Vector3(x * gridSize, height * gridSize, 0),
                       parent.transform);
        }

        for (int y = 0; y <= height; y++)
        {
            CreateLine(origin + new Vector3(0, y * gridSize, 0),
                       origin + new Vector3(width * gridSize, y * gridSize, 0),
                       parent.transform);
        }
    }

    void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(parent);
        var lr = lineObj.AddComponent<LineRenderer>();

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.sortingOrder = 100;
    }
}
