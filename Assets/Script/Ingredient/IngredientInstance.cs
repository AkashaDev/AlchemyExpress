using System.Collections.Generic;
using UnityEngine;

public class IngredientInstance : MonoBehaviour
{
    [Header("Data")]
    public IngredientSO data;

    [Header("Visual")]
    public GameObject cellPrefab;          // Prefab untuk grid putih
    public Sprite cellSprite;              // Sprite visual untuk grid cell
    public Transform spriteObject;         // Sprite utama (SpriteVisual)

    private List<GameObject> renderedCells = new List<GameObject>();
    private int rotationIndex = 0;

    public void Setup(IngredientSO ingredient)
    {
        data = ingredient;
        rotationIndex = 0;

        // Set sprite utama ke SpriteVisual
        if (spriteObject != null)
        {
            SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = data.itemSprite;
        }

        Redraw();
        UpdateSpriteVisualTransform();
    }

    public void RotateClockwise()
    {
        rotationIndex = (rotationIndex + 1) % 4;
        Redraw();
        UpdateSpriteVisualTransform();
    }

    public void Redraw()
    {
        // Hapus cell sebelumnya
        foreach (var cell in renderedCells)
            Destroy(cell);
        renderedCells.Clear();

        // Dapatkan shape setelah rotasi
        Vector2Int[] rotatedShape = GetRotatedShape(rotationIndex);

        // Gambar ulang cell
        foreach (var cellPos in rotatedShape)
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
        // Mapping rotasi khusus untuk bentuk seperti L
        switch (rotationIndex % 4)
        {
            case 0: return Quaternion.Euler(0, 0, 0);
            case 1: return Quaternion.Euler(0, 0, -270); // swap dengan -90
            case 2: return Quaternion.Euler(0, 0, -180);
            case 3: return Quaternion.Euler(0, 0, -90);  // swap dengan -270
            default: return Quaternion.identity;
        }
    }
}
