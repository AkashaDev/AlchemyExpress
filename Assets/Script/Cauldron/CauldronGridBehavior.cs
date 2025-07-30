using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronGridBehavior : MonoBehaviour
{
    public int width;
    public int height;
    public Vector3 origin;
    private SpriteRenderer[,] tileRenderers;

    [SerializeField]
    public List<int> debugGrid;

    private int[,] gridStatus;
    private IngredientInstance[,] gridOwner;
    private List<IngredientInstance> placedIngredients = new List<IngredientInstance>();

    [SerializeField]
    private List<string> debugIngredientNames = new List<string>();

    public void Initialize(int width, int height, Vector3 origin, bool[] blockedTiles)
    {
        this.width = width;
        this.height = height;
        this.origin = origin;

        gridStatus = new int[width, height];
        gridOwner = new IngredientInstance[width, height];
        debugGrid = new List<int>(new int[width * height]);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int value = 0;
                if (blockedTiles != null && blockedTiles[y * width + x])
                    value = -1;

                gridStatus[x, y] = value;
                debugGrid[y * width + x] = value;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        Vector2 local = worldPos - (Vector2)origin;
        return new Vector2Int(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return origin + new Vector3(gridPos.x, gridPos.y, 0);
    }

    public bool CanPlaceIngredient(IngredientInstance inst, Vector2Int pivot)
    {
        Vector2Int[] occupied = inst.GetOccupiedGridPositions(pivot);

        foreach (var pos in occupied)
        {
            if (!IsInsideGrid(pos))
                return false;
            if (gridStatus[pos.x, pos.y] != 0)
                return false;
        }

        return true;
    }

    public void PlaceIngredient(IngredientInstance inst, Vector2Int pivot)
    {
        foreach (var pos in inst.GetOccupiedGridPositions(pivot))
        {
            gridStatus[pos.x, pos.y] = 1;
            gridOwner[pos.x, pos.y] = inst;
            debugGrid[pos.y * width + pos.x] = 1;
        }
        placedIngredients.Add(inst);
        UpdateDebugIngredientNames();
    }

    public void RemoveIngredient(IngredientInstance inst)
    {
        if (!placedIngredients.Contains(inst))
            return;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (gridOwner[x, y] == inst)
                {
                    gridStatus[x, y] = 0;
                    gridOwner[x, y] = null;
                    debugGrid[y * width + x] = 0;
                }
            }
        }

        placedIngredients.Remove(inst);
        UpdateDebugIngredientNames();
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void ClearPreview()
    {
        if (tileRenderers == null)
            return;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (gridStatus[x, y] == -1)
                    tileRenderers[x, y].color = Color.black;
                else
                    tileRenderers[x, y].color = Color.white;
            }
        }
    }

    public void PreviewPlacement(Vector2Int[] shape, Vector2Int pivot, bool valid)
    {
        if (tileRenderers == null)
            return;

        foreach (var offset in shape)
        {
            Vector2Int pos = pivot + offset;
            if (IsInsideGrid(pos))
            {
                var renderer = tileRenderers[pos.x, pos.y];
                renderer.color = valid ? Color.green : Color.red;
            }
        }
    }

    public void SetTileRenderers(SpriteRenderer[,] renderers)
    {
        tileRenderers = renderers;
    }

    private SpriteRenderer GetTileSpriteRenderer(int x, int y)
    {
        return null;
    }

    public List<IngredientInstance> GetCurrentIngredients()
    {
        return new List<IngredientInstance>(placedIngredients);
    }

    public List<string> GetIngredientNamesInCauldron()
    {
        return placedIngredients
            .Where(p => p != null && p.data != null)
            .Select(p => p.data.ingredientName)
            .ToList();
    }

    private void UpdateDebugIngredientNames()
    {
        debugIngredientNames = placedIngredients
            .Where(p => p != null && p.data != null)
            .Select(p => p.data.ingredientName)
            .ToList();
    }

    public void ClearAll()
    {
        // Hapus semua object dari scene
        foreach (var ingredient in placedIngredients)
        {
            if (ingredient != null)
            {
                Destroy(ingredient.gameObject);
            }
        }

        // Kosongkan data grid internal jika kamu punya (misalnya array 2D)
        // gridArray[x, y] = null; ← kamu bersihkan juga jika pakai sistem grid seperti itu

        // Kosongkan list tracking
        placedIngredients.Clear();

        // Optional: bersihkan preview jika masih muncul
        ClearPreview();

        Debug.Log(" Cauldron dibersihkan.");
    }

    public bool IsIngredientPlaced(IngredientInstance inst)
    {
        if (inst == null)
            return false;

        // Cukup periksa apakah item tersebut ada di dalam daftar pelacakan kita.
        return placedIngredients.Contains(inst);
    }
}
