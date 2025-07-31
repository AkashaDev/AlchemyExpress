using UnityEngine;
using UnityEngine.UI;

public class RecipeBookButton : MonoBehaviour
{
    [Header("UI References")]
    public Button recipeBookButton;
    public GameObject recipeBookUI; // Panel untuk pop-up
    
    [Header("Data")]
    public RecipeBookData recipeBookData;
    
    private void Start()
    {
        // Setup button di pojok kiri
        recipeBookButton.onClick.AddListener(OpenRecipeBook);
        
        // Pastikan pop-up tertutup saat start
        if(recipeBookUI != null)
            recipeBookUI.SetActive(false);
    }
    
    public void OpenRecipeBook()
    {
        if(recipeBookUI != null)
        {
            recipeBookUI.SetActive(true);
            
            // Update recipe book content
            RecipeBookUI recipeUI = recipeBookUI.GetComponent<RecipeBookUI>();
            if(recipeUI != null)
            {
                recipeUI.DisplayRecipes(recipeBookData.GetAllPotions());
            }
        }
    }
    
    public void CloseRecipeBook()
    {
        if(recipeBookUI != null)
            recipeBookUI.SetActive(false);
    }
}