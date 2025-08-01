using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class IngredientInstance : MonoBehaviour
{
    public IngredientSO data;
    public GameObject cellPrefab;
    public Sprite cellSprite;
    public Transform spriteObject;

    private IngredientRuntimeData runtimeData;
    private List<GameObject> renderedCells = new List<GameObject>();

    private int rotationIndex = 0;

    private Vector3 originalSpawnPosition;
    private Transform originalParent;
    private int originalRotationIndex = 0;

    private bool hasEverBeenPlacedInCauldron = false;
    private Vector3 lastCauldronWorldPos;
    private int lastCauldronRotationIndex;
    public bool IsBeingDragged { get; set; }

    // === Public API ===

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

        Redraw();
        UpdateSpriteVisualTransform();
    }

    public void RememberSpawnPosition()
    {
        originalSpawnPosition = transform.position;
        originalParent = transform.parent;
        originalRotationIndex = 0;
    }

    public void ResetToSpawnPoint()
    {
        transform.SetParent(originalParent);
        transform.position = originalSpawnPosition;
        transform.rotation = Quaternion.identity;
        SetRotationIndex(originalRotationIndex);
    }

    public void RememberPlacedInCauldron(Vector3 worldPos, int rotationIndex)
    {
        hasEverBeenPlacedInCauldron = true;
        lastCauldronWorldPos = worldPos;
        lastCauldronRotationIndex = rotationIndex;
    }

    public bool HasEverBeenPlaced => hasEverBeenPlacedInCauldron;

    public void ReturnToLastKnownValidPosition(CauldronSpawner cauldron)
    {
        if (hasEverBeenPlacedInCauldron)
        {
            transform.position = lastCauldronWorldPos;
            SetRotationIndex(lastCauldronRotationIndex);
            transform.SetParent(cauldron.tileParent);
        }
        else
        {
            ResetToSpawnPoint();
        }
    }

    public void RotateClockwise()
    {
        if (!runtimeData.canRotate) return;

        rotationIndex = (rotationIndex + 1) % 4;
        Redraw();
        UpdateSpriteVisualTransform();
    }

    public int GetRotationIndex() => rotationIndex;

    public void SetRotationIndex(int index)
    {
        rotationIndex = index % 4;
        Redraw();
        UpdateSpriteVisualTransform();
    }

    public Vector2Int[] GetRotatedShapeForExternal()
    {
        return runtimeData.GetRotatedShape(rotationIndex);
    }

    public Vector2 GetPivotWorldPosition()
    {
        return transform.position;
    }

    public Vector2Int[] GetOccupiedGridPositions(Vector2Int pivot)
    {
        var shape = GetRotatedShapeForExternal();
        Vector2Int[] result = new Vector2Int[shape.Length];

        for (int i = 0; i < shape.Length; i++)
            result[i] = pivot + shape[i];

        return result;
    }

    // === Rendering & Collision ===

    private void Redraw()
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
            Vector2[] square = new Vector2[4]
            {
                new Vector2(pos.x, pos.y),
                new Vector2(pos.x + 1, pos.y),
                new Vector2(pos.x + 1, pos.y + 1),
                new Vector2(pos.x, pos.y + 1)
            };
            polygon.SetPath(i, square);
        }
    }

    private void UpdateSpriteVisualTransform()
    {
        Vector2Int[] shape = runtimeData.GetRotatedShape(rotationIndex);
        Vector2 avg = Vector2.zero;
        foreach (var pos in shape) avg += (Vector2)pos;
        avg /= shape.Length;

        if (spriteObject != null)
        {
            spriteObject.localPosition = avg;
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

    
}
