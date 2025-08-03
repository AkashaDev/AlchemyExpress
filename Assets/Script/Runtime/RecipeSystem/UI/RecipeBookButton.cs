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
        recipeBookButton.onClick.AddListener(OpenRecipeBook);
        
        if(recipeBookUI != null)
            recipeBookUI.SetActive(false);
    }
    
    public void OpenRecipeBook()
    {
        if(recipeBookUI != null)
        {
            recipeBookUI.SetActive(true);
            
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