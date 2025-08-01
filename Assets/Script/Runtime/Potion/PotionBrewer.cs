using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionBrewer : MonoBehaviour
{
    public CauldronGridBehavior cauldron;
    // public List<RecipeSO> allRecipes;
    public GameObject potionPrefab;
    public Transform spawnPoint;

    [Header("Referensi Buku Resep")]
    public RecipeBookData recipeBook;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryBrewPotion();
        }
    }

    private void TryBrewPotion()
    {
        if (recipeBook == null)
        {
            Debug.LogError("RecipeBookData belum di-assign di PotionBrewer!");
            return;
        }

        List<IngredientInstance> ingredientsInCauldron = cauldron.GetCurrentIngredients();

        foreach (Potion potionRecipe in recipeBook.allPotions)
        {
            if (IsMatch(potionRecipe, ingredientsInCauldron))
            {
                Debug.Log("Resep cocok! Menghasilkan: " + potionRecipe.potionName);

                SpawnPotionObject(potionRecipe);
                cauldron.ClearAll();

                return;
            }
        }

        Debug.Log("Tidak cocok dengan resep manapun.");
    }

    bool IsMatch(Potion recipe, List<IngredientInstance> placedIngredients)
    {
        List<string> requiredNames = recipe
            .requiredIngredients.Select(ingredient => ingredient.ingredientName)
            .ToList();

        List<string> placedNames = placedIngredients
            .Where(i => i != null && i.data != null)
            .Select(i => i.data.ingredientName)
            .ToList();

        if (requiredNames.Count != placedNames.Count)
        {
            return false;
        }

        requiredNames.Sort();
        placedNames.Sort();

        return requiredNames.SequenceEqual(placedNames);
    }

    private void SpawnPotionObject(Potion potion)
    {
        GameObject go = Instantiate(potionPrefab, spawnPoint != null ? spawnPoint.position : Vector3.zero, Quaternion.identity);
        go.name = "Potion_" + potion.potionName;

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = potion.potionImage;
            sr.sortingLayerName = "Gameplay";
            sr.sortingOrder = 100;
        }

        PotionDragHandler drag = go.GetComponent<PotionDragHandler>();
        if (drag != null)
        {
            drag.potionData = potion;
        }
    }
}
