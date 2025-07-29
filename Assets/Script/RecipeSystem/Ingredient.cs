using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Recipe System/Ingredient")]
public class Ingredient : ScriptableObject
{
    [Header("Ingredient Info")]
    public int ingredientId;
    public string ingredientName;
    public Sprite ingredientImage;
    [TextArea(2, 4)]
    public string description;
}