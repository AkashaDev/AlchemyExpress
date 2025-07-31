using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RecipeBookUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform recipeListParent; // Parent untuk list resep
    public GameObject recipePrefab; // Prefab untuk setiap resep
    public Button closeButton;
    
    [Header("Recipe Detail Panel")]
    public GameObject recipeDetailPanel;
    public Image potionImageDetail;
    public TextMeshProUGUI potionNameDetail;
    public TextMeshProUGUI potionDescriptionDetail;
    public TextMeshProUGUI potionEffectDetail;
    public Transform ingredientsListParent;
    public GameObject ingredientPrefab;
    
    private List<GameObject> currentRecipeItems = new List<GameObject>();
    private List<GameObject> currentIngredientItems = new List<GameObject>();
    
    private void Start()
    {
        if(closeButton != null)
            closeButton.onClick.AddListener(CloseRecipeBook);
            
        if(recipeDetailPanel != null)
            recipeDetailPanel.SetActive(false);
    }
    
    public void DisplayRecipes(List<Potion> potions)
    {
        // Clear existing recipe items
        ClearRecipeList();
        
        // Create recipe items
        foreach(Potion potion in potions)
        {
            GameObject recipeItem = Instantiate(recipePrefab, recipeListParent);
            
            // Setup recipe item
            RecipeItem recipeItemScript = recipeItem.GetComponent<RecipeItem>();
            if(recipeItemScript != null)
            {
                recipeItemScript.SetupRecipe(potion, this);
            }
            
            currentRecipeItems.Add(recipeItem);
        }
    }
    
    public void ShowRecipeDetail(Potion potion)
    {
        if(recipeDetailPanel != null)
        {
            recipeDetailPanel.SetActive(true);
            
            // Set potion info
            if(potionImageDetail != null)
                potionImageDetail.sprite = potion.potionImage;
                
            if(potionNameDetail != null)
                potionNameDetail.text = potion.potionName;
                
            if(potionDescriptionDetail != null)
                potionDescriptionDetail.text = potion.potionDescription;
                
            if(potionEffectDetail != null)
                potionEffectDetail.text = "Kegunaan: " + potion.potionEffect;
            
            // Display ingredients
            DisplayIngredients(potion);
        }
    }
    
    private void DisplayIngredients(Potion potion)
    {
        // Clear existing ingredients
        ClearIngredientsList();
        
        // Create ingredient items
        for(int i = 0; i < potion.requiredIngredients.Length; i++)
        {
            if(potion.requiredIngredients[i] != null)
            {
                GameObject ingredientItem = Instantiate(ingredientPrefab, ingredientsListParent);
                
                IngredientItem ingredientScript = ingredientItem.GetComponent<IngredientItem>();
                if(ingredientScript != null)
                {
                    int quantity = i < potion.ingredientQuantities.Length ? potion.ingredientQuantities[i] : 1;
                    ingredientScript.SetupIngredient(potion.requiredIngredients[i], quantity);
                }
                
                currentIngredientItems.Add(ingredientItem);
            }
        }
    }
    
    public void CloseRecipeDetail()
    {
        if(recipeDetailPanel != null)
            recipeDetailPanel.SetActive(false);
    }
    
    public void CloseRecipeBook()
    {
        gameObject.SetActive(false);
    }
    
    private void ClearRecipeList()
    {
        foreach(GameObject item in currentRecipeItems)
        {
            if(item != null)
                DestroyImmediate(item);
        }
        currentRecipeItems.Clear();
    }
    
    private void ClearIngredientsList()
    {
        foreach(GameObject item in currentIngredientItems)
        {
            if(item != null)
                DestroyImmediate(item);
        }
        currentIngredientItems.Clear();
    }
}