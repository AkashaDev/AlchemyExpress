using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronSpawner : MonoBehaviour
{
    public CauldronSO cauldronTemplate;
    public GameObject tilePrefab;
    public Transform tileParent;

    private CauldronRuntimeData runtimeData;
    private Dictionary<Vector2Int, IngredientInstance> occupiedTiles = new Dictionary<Vector2Int, IngredientInstance>();
    private List<IngredientInstance> ingredientsInCauldron = new List<IngredientInstance>();
    /*private Dictionary<Vector2Int, Transform> tileLookup = new Dictionary<Vector2Int, Transform>();*/
    private Dictionary<Vector2Int, CauldronTile> tileLookup = new Dictionary<Vector2Int, CauldronTile>();
    void Start()
    {
        GenerateCauldron();
    }

    public void GenerateCauldron()
    {
        runtimeData = new CauldronRuntimeData(cauldronTemplate);
        occupiedTiles.Clear();
        ingredientsInCauldron.Clear();
        tileLookup.Clear();

        for (int y = 0; y < runtimeData.height; y++)
        {
            for (int x = 0; x < runtimeData.width; x++)
            {
                GameObject tile = Instantiate(tilePrefab, tileParent);
                tile.transform.localPosition = new Vector3(x, y, 0);

                var tileScript = tile.GetComponent<CauldronTile>();
                if (tileScript != null)
                {
                    bool blocked = runtimeData.IsBlocked(x, y);
                    tileScript.SetColor(blocked ? Color.black : Color.white);
                    tileLookup[new Vector2Int(x, y)] = tileScript;
                }
            }
        }
    }

    public Transform GetTileTransformAt(Vector2Int pos)
    {
        if (tileLookup.ContainsKey(pos))
            return tileLookup[pos].transform;
        return null;
    }

    public bool CanAcceptIngredient(IngredientInstance ing, Vector2Int basePos)
    {
        foreach (var cell in ing.GetRotatedShapeForExternal())
        {
            Vector2Int checkPos = cell + basePos;

            if (checkPos.x < 0 || checkPos.x >= runtimeData.width || checkPos.y < 0 || checkPos.y >= runtimeData.height)
                return false;

            if (runtimeData.IsBlocked(checkPos.x, checkPos.y)) return false;
            if (occupiedTiles.ContainsKey(checkPos)) return false;
        }
        return true;
    }

    public void RegisterIngredient(IngredientInstance ing, Vector2Int basePos)
    {
        foreach (var cell in ing.GetRotatedShapeForExternal())
        {
            Vector2Int pos = cell + basePos;
            occupiedTiles[pos] = ing;
        }

        if (!ingredientsInCauldron.Contains(ing))
            ingredientsInCauldron.Add(ing);
    }

    public void Brew()
    {
        if (CheckRecipeMatch())
        {
            Debug.Log("Potion berhasil dibuat!");
        }
        else
        {
            Debug.LogWarning("Potion gagal! Meledak!");
            ResetCauldron();
        }
    }

    private bool CheckRecipeMatch()
    {
        return ingredientsInCauldron.Count == 2; // contoh dummy
    }

    public void ResetCauldron()
    {
        foreach (var ing in ingredientsInCauldron)
        {
            Destroy(ing.gameObject);
        }

        occupiedTiles.Clear();
        ingredientsInCauldron.Clear();
    }

    public void PreviewPlacement(Vector2Int[] shape, Vector2Int gridPos, bool isValid)
    {
        foreach (var cell in shape)
        {
            Vector2Int pos = cell + gridPos;
            if (tileLookup.ContainsKey(pos))
            {
                tileLookup[pos].SetColor(isValid ? Color.green : Color.red);
            }
        }
    }

    public void ClearPreview()
    {
        foreach (var tile in tileLookup.Values)
        {
            tile.ResetColor();
        }
    }

    public void UnregisterIngredient(IngredientInstance ing)
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var kvp in occupiedTiles)
        {
            if (kvp.Value == ing)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var pos in toRemove)
        {
            occupiedTiles.Remove(pos);
        }
    }

}
