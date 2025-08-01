using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionBrewer : MonoBehaviour
{
    public CauldronGridBehavior cauldron;
    public List<RecipeSO> allRecipes;
    public GameObject potionPrefab;
    public Transform spawnPoint;

    private void Update()
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

                SpawnPotionObject(recipe.resultPotion);

                cauldron.ClearAll();

                return;
            }
        }

        Debug.Log("Tidak cocok dengan resep manapun.");
    }

   private bool IsMatch(RecipeSO recipe, List<IngredientInstance> placed)
    {
        List<string> names = placed
            .Where(i => i != null && i.data != null)
            .Select(i => i.data.ingredientName)
            .ToList();

        return recipe.requiredIngredientsName.All(r => names.Contains(r))
            && names.Count == recipe.requiredIngredientsName.Count;
    }

   private void SpawnPotionObject(PotionSO potion)
    {
        GameObject go = Instantiate(potionPrefab, spawnPoint != null ? spawnPoint.position : Vector3.zero, Quaternion.identity);
        go.name = "Potion_" + potion.potionName;

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = potion.icon;
            sr.sortingLayerName = "Gameplay";
            sr.sortingOrder = 10;
        }

        PotionDragHandler drag = go.GetComponent<PotionDragHandler>();
        if (drag != null)
        {
            drag.potionData = potion;
        }
    }

}
