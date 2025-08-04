using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Recipe System/Potion")]
public class Potion : ScriptableObject
{
    [Header("Potion Info")]
    public int potionId;
    public string potionName;
    public Sprite potionImage;
    public Sprite potionImageBook;
    [TextArea(3, 6)]
    public string potionDescription;
    public string potionEffect; // Kegunaan potion
    
    [Header("Recipe")]
    public IngredientSO[] requiredIngredients;
}