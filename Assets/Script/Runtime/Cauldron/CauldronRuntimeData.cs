
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientPlacementData
{
    public IngredientSO ingredientSO;
    public Vector2Int pivotPosition;
    public int rotationIndex;
}

[CreateAssetMenu(fileName = "CauldronRuntimeData", menuName = "Potion/Cauldron Runtime Data")]
public class CauldronRuntimeData : ScriptableObject
{
    [Header("Grid Info")]
    public int width;
    public int height;

    [Tooltip("Tentukan posisi tile yang terblokir (y * width + x)")]
    public List<bool> blockedTiles = new List<bool>();

    [Header("Ingredient Placement")]
    public List<IngredientPlacementData> placedIngredients = new List<IngredientPlacementData>();

    public void Clear()
    {
        placedIngredients.Clear();
    }

    public bool IsTileBlocked(int x, int y)
    {
        int index = y * width + x;
        if (index >= 0 && index < blockedTiles.Count)
        {
            return blockedTiles[index];
        }
        return false;
    }
}
