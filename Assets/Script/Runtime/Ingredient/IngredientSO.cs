using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Potion/Ingredient")]
public class IngredientSO : ScriptableObject
{
    public string ingredientName;
    public Sprite itemSprite;

    public bool canRotate = true;
    public Vector2Int[] shapeCells;
}
