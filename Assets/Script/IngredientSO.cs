using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Potion/Ingredient")]
public class IngredientSO : ScriptableObject
{
    public string ingredientName;
    public Sprite sprite; // Gambar utama (misalnya bunga tulip)
    public bool canRotate = true;

    // Posisi blok relatif (dalam grid)
    public Vector2Int[] shapeCells;
}
