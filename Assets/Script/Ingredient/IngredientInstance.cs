using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class IngredientInstance : MonoBehaviour
{
    public IngredientSO data; // referensi SO, tetap ada
    public GameObject cellPrefab;
    public Sprite cellSprite;
    public Transform spriteObject;

    private IngredientRuntimeData runtimeData;
    private int rotationIndex = 0;
    private List<GameObject> renderedCells = new List<GameObject>();

    public void Setup(IngredientSO source)
    {
        data = source;
        runtimeData = new IngredientRuntimeData(source);
        rotationIndex = 0;

        if (spriteObject != null)
        {
            SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = data.itemSprite;
        }
        Debug.Log("[IngredientInstance] Setup called with: " + source.name);
        Redraw();
        UpdateSpriteVisualTransform();
    }

    public void RotateClockwise()
    {
        if (!runtimeData.canRotate) return;

        rotationIndex = (rotationIndex + 1) % 4;
        Redraw();
        UpdateSpriteVisualTransform();
    }

    public void Redraw()
    {
        foreach (var cell in renderedCells)
            Destroy(cell);
        renderedCells.Clear();

        Vector2Int[] shape = runtimeData.GetRotatedShape(rotationIndex);

        foreach (var cellPos in shape)
        {
            GameObject cell = Instantiate(cellPrefab, transform);
            cell.transform.localPosition = new Vector3(cellPos.x, cellPos.y, 0);

            SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
            if (sr != null && cellSprite != null)
            {
                sr.sprite = cellSprite;
                sr.sortingOrder = -1;
            }

            renderedCells.Add(cell);
        }

        UpdatePolygonCollider(shape);
    }

    private void UpdatePolygonCollider(Vector2Int[] shape)
    {
        PolygonCollider2D polygon = GetComponent<PolygonCollider2D>();
        if (polygon == null) return;

        polygon.pathCount = shape.Length;

        for (int i = 0; i < shape.Length; i++)
        {
            Vector2Int pos = shape[i];

            Vector2[] square = new Vector2[4];
            square[0] = new Vector2(pos.x, pos.y);
            square[1] = new Vector2(pos.x + 1, pos.y);
            square[2] = new Vector2(pos.x + 1, pos.y + 1);
            square[3] = new Vector2(pos.x, pos.y + 1);

            polygon.SetPath(i, square);
        }
    }

    public Vector2Int[] GetRotatedShapeForExternal()
    {
        return runtimeData.GetRotatedShape(rotationIndex);
    }

    private Vector2Int[] GetRotatedShape(int rotation)
    {
        Vector2Int[] rotated = new Vector2Int[data.shapeCells.Length];

        for (int i = 0; i < data.shapeCells.Length; i++)
        {
            Vector2Int c = data.shapeCells[i];

            switch (rotation % 4)
            {
                case 0: rotated[i] = c; break;
                case 1: rotated[i] = new Vector2Int(-c.y, c.x); break;
                case 2: rotated[i] = new Vector2Int(-c.x, -c.y); break;
                case 3: rotated[i] = new Vector2Int(c.y, -c.x); break;
            }
        }

        return rotated;
    }


    private void UpdateSpriteVisualTransform()
    {
        Vector2Int[] shape = GetRotatedShape(rotationIndex);

        // Hitung posisi tengah shape untuk memusatkan sprite
        Vector2 avg = Vector2.zero;
        foreach (var pos in shape)
            avg += (Vector2)pos;
        avg /= shape.Length;

        if (spriteObject != null)
        {
            // Posisikan sprite utama di tengah shape
            spriteObject.localPosition = avg;

            // Atur rotasi sprite visual
            spriteObject.localRotation = GetSpriteRotation(rotationIndex);
            spriteObject.localScale = Vector3.one;
        }
    }
    private Quaternion GetSpriteRotation(int rotationIndex)
    {
        switch (rotationIndex % 4)
        {
            case 0: return Quaternion.Euler(0, 0, 0);
            case 1: return Quaternion.Euler(0, 0, -270);
            case 2: return Quaternion.Euler(0, 0, -180);
            case 3: return Quaternion.Euler(0, 0, -90);
            default: return Quaternion.identity;
        }
    }
    public int GetRotationIndex()
    {
        return rotationIndex;
    }

    public Vector2 GetPivotWorldPosition()
    {
        Vector2Int[] shape = GetRotatedShapeForExternal();

        // Cari sel (0,0) dalam bentuk lokal
        foreach (var cell in shape)
        {
            if (cell == Vector2Int.zero)
            {
                // Posisi pivot (0,0) di world space = posisi objek + offset sel (0,0)
                Vector3 worldPos = transform.position;
                return new Vector2(Mathf.Round(worldPos.x), Mathf.Round(worldPos.y));
            }
        }

        // fallback jika tidak ada (seharusnya tidak terjadi)
        return transform.position;
    }

    public void SetRotationIndex(int index)
    {
        rotationIndex = index % 4;
        Redraw();
        UpdateSpriteVisualTransform();
    }
}
