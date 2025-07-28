using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Potion/Ingredient")]
public class IngredientSO : ScriptableObject
{
    [Header("Basic Info")]
    public string ingredientName;
    public Sprite itemSprite;

    [Header("Shape & Rotation")]
    public bool canRotate = true;
    public Vector2Int[] shapeCells;
    public Vector2Int[] GetRotatedShape(int rotation)
    {
        Vector2Int[] rotated = new Vector2Int[shapeCells.Length];
        for (int i = 0; i < shapeCells.Length; i++)
        {
            Vector2Int c = shapeCells[i];
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

    [Header("Metadata")]
    public List<string> aliases;
    public bool isTrashable = true;
}
