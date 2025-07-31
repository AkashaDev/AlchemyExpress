using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronGridBehavior : MonoBehaviour
{
    public int width;
    public int height;
    public Vector3 origin;

    private int xOffset;
    private int yOffset;

    private SpriteRenderer[,] tileRenderers;
    [SerializeField] public List<int> debugGrid;

    private int[,] gridStatus;
    private IngredientInstance[,] gridOwner;
    private List<IngredientInstance> placedIngredients = new List<IngredientInstance>();
    [SerializeField] private List<string> debugIngredientNames = new List<string>();

    public void Initialize(int width, int height, Vector3 origin, bool[] blockedTiles)
    {
        this.width = width;
        this.height = height;

        // Perubahan: Sesuaikan origin untuk grid berukuran genap
        this.origin = origin;
        if (width % 2 == 0) // Jika lebar genap
            this.origin.x += 0.5f;
        if (height % 2 == 0) // Jika tinggi genap
            this.origin.y += 0.5f;

        // ... kode existing di bawah tetap sama ...
        xOffset = width / 2;
        yOffset = height / 2;

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

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return origin + new Vector3(gridPos.x, gridPos.y, 0);
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        Vector2 local = worldPos - (Vector2)origin;
        return new Vector2Int(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
    }

    private bool TryGetArrayIndex(Vector2Int gridPos, out int x, out int y)
    {
        x = gridPos.x + xOffset;
        y = gridPos.y + yOffset;
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool CanPlaceIngredient(IngredientInstance inst, Vector2Int pivot)
    {
        Vector2Int[] occupied = inst.GetOccupiedGridPositions(pivot);

        foreach (var pos in occupied)
        {
            if (!TryGetArrayIndex(pos, out int x, out int y)) return false;
            if (gridStatus[x, y] != 0) return false;
        }

        return true;
    }

    public void PlaceIngredient(IngredientInstance inst, Vector2Int pivot)
    {
        foreach (var pos in inst.GetOccupiedGridPositions(pivot))
        {
            if (TryGetArrayIndex(pos, out int x, out int y))
            {
                gridStatus[x, y] = 1;
                gridOwner[x, y] = inst;
                debugGrid[y * width + x] = 1;
            }
        }
        placedIngredients.Add(inst);
        UpdateDebugIngredientNames();
    }

    public void RemoveIngredient(IngredientInstance inst)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (gridOwner[x, y] != null && gridOwner[x, y].Equals(inst))
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
        return TryGetArrayIndex(pos, out _, out _);
    }

    public void ClearPreview()
    {
        if (tileRenderers == null) return;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (gridStatus[x, y] == -1)
                    tileRenderers[x, y].color = Color.black;
                else
                    tileRenderers[x, y].color = new Color(1f, 1f, 1f, 0.3f);
            }
        }
    }

    public void PreviewPlacement(Vector2Int[] shape, Vector2Int pivot, bool valid)
    {
        if (tileRenderers == null) return;

        foreach (var offset in shape)
        {
            Vector2Int pos = pivot + offset;
            if (TryGetArrayIndex(pos, out int x, out int y))
            {
                var renderer = tileRenderers[x, y];
                renderer.color = valid ? Color.green : Color.red;
            }
        }
    }

    public void SetTileRenderers(SpriteRenderer[,] renderers)
    {
        tileRenderers = renderers;
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
        foreach (var ingredient in placedIngredients.ToList())
        {
            if (ingredient != null)
            {
                RemoveIngredient(ingredient);
                Destroy(ingredient.gameObject);
            }
        }

        placedIngredients.Clear();
        debugIngredientNames.Clear();
        ClearPreview();
    }

    public bool IsIngredientPlaced(IngredientInstance inst)
    {
        if (inst == null)
            return false;

        return placedIngredients.Contains(inst);
    }
}
