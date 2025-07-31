using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionBrewer : MonoBehaviour
{
    public CauldronGridBehavior cauldron;
    public List<RecipeSO> allRecipes;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryBrewPotion();
        }
    }

    private void TryBrewPotion() 
    {
        List<IngredientInstance> ingredients = cauldron.GetCurrentIngredients();

        foreach (RecipeSO recipe in allRecipes)
        {
            if (IsMatch(recipe, ingredients))
            {
                Debug.Log(" Resep cocok! Menghasilkan: " + recipe.resultPotion.potionName);

                // Tampilkan hasil potion secara visual
                SpawnPotionObject(recipe.resultPotion);

                // Bersihkan cauldron
                cauldron.ClearAll();

                return;
            }
        }

        Debug.Log("Tidak cocok dengan resep manapun.");
    }

    bool IsMatch(RecipeSO recipe, List<IngredientInstance> placed)
    {
        List<string> names = placed
            .Where(i => i != null && i.data != null)
            .Select(i => i.data.ingredientName)
            .ToList();

        return recipe.requiredIngredientsName.All(r => names.Contains(r))
            && names.Count == recipe.requiredIngredientsName.Count;
    }

    void SpawnPotionObject(PotionSO potion)
    {
        // Ini hanya contoh logika visual
        GameObject go = new GameObject("Potion_" + potion.potionName);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = potion.icon;
        sr.sortingOrder = 10;

        go.transform.position = new Vector3(0, 0, 0);
    }
}
