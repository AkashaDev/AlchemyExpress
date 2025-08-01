using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientItem : MonoBehaviour
{
    [Header("UI References")]
    public Image ingredientImage;
    public TextMeshProUGUI ingredientName;
    public TextMeshProUGUI ingredientQuantity;
    
    public void SetupIngredient(IngredientSO ingredient, int quantity)
    {
        if(ingredientImage != null)
            ingredientImage.sprite = ingredient.itemIcon;
            
        if(ingredientName != null)
            ingredientName.text = ingredient.ingredientName;
            
        if(ingredientQuantity != null)
            ingredientQuantity.text = "x" + quantity.ToString();
    } }