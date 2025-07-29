using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItem : MonoBehaviour
{
    [Header("UI References")]
    public Image potionImage;
    public TextMeshProUGUI potionName;
    public Button selectButton;
    
    private Potion currentPotion;
    private RecipeBookUI recipeBookUI;
    
    public void SetupRecipe(Potion potion, RecipeBookUI bookUI)
    {
        currentPotion = potion;
        recipeBookUI = bookUI;
        
        if(potionImage != null)
            potionImage.sprite = potion.potionImage;
            
        if(potionName != null)
            potionName.text = potion.potionName;
            
        if(selectButton != null)
            selectButton.onClick.AddListener(OnRecipeSelected);
    }
    
    private void OnRecipeSelected()
    {
        if(recipeBookUI != null && currentPotion != null)
        {
            recipeBookUI.ShowRecipeDetail(currentPotion);
        }
    }
}