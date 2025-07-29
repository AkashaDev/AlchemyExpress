using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronSpawner : MonoBehaviour
{
    public CauldronTemplateSO template;
    public GameObject tilePrefab;
    public Transform tileParent;

    private Transform[,] grid;
    private IngredientInstance[,] placedIngredients;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        if (template == null) return;

        grid = new Transform[template.width, template.height];
        placedIngredients = new IngredientInstance[template.width, template.height];

        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                Vector3 worldPos = tileParent.position + new Vector3(x, y, 0);
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, tileParent);

                if (template.IsBlocked(x, y))
                    tile.GetComponent<SpriteRenderer>().color = Color.black;

                grid[x, y] = tile.transform;
            }
        }
    }

    public bool CanAcceptIngredient(IngredientInstance inst, Vector2Int pivot)
    {
        Vector2Int[] positions = inst.GetOccupiedGridPositions(pivot);

        foreach (var pos in positions)
        {
            if (!IsInsideGrid(pos)) return false;
            if (template.IsBlocked(pos.x, pos.y)) return false;
            if (placedIngredients[pos.x, pos.y] != null) return false;
        }

        return true;
    }

    public void RegisterIngredient(IngredientInstance inst, Vector2Int pivot)
    {
        foreach (var pos in inst.GetOccupiedGridPositions(pivot))
        {
            placedIngredients[pos.x, pos.y] = inst;
        }
    }

    public void UnregisterIngredient(IngredientInstance inst)
    {
        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                if (placedIngredients[x, y] == inst)
                    placedIngredients[x, y] = null;
            }
        }
    }

    public Transform GetTileTransformAt(Vector2Int pos)
    {
        if (!IsInsideGrid(pos)) return null;
        return grid[pos.x, pos.y];
    }

    public void ClearPreview()
    {
        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                var sr = grid[x, y].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = template.IsBlocked(x, y) ? Color.black : Color.white;
                }
            }
        }
    }

    public void PreviewPlacement(Vector2Int[] shape, Vector2Int pivot, bool valid)
    {
        foreach (var offset in shape)
        {
            Vector2Int pos = pivot + offset;
            if (IsInsideGrid(pos))
            {
                var sr = grid[pos.x, pos.y].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = valid ? Color.green : Color.red;
                }
            }
        }
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPos)
    {
        Vector2 local = worldPos - (Vector2)tileParent.position;
        return new Vector2Int(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < template.width && pos.y >= 0 && pos.y < template.height;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (template == null || tileParent == null) return;

        Gizmos.color = Color.white;
        Vector3 offset = tileParent.position;

        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                Vector3 pos = offset + new Vector3(x, y, 0);
                Vector3 size = Vector3.one;

                if (template.IsBlocked(x, y))
                {
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Gizmos.DrawCube(pos, size);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(pos, size);
                }
                else
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireCube(pos, size);
                }
            }
        }
    }
#endif
}
