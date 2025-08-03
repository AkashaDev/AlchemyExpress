using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using ObeserverPattern;

public class PotionBrewer : MonoBehaviour
{
    public CauldronGridBehavior cauldron;
    // public List<RecipeSO> allRecipes;
    public GameObject potionPrefab;
    public Transform spawnPoint;
    public Transform dotBetweenTransform;
    private bool isBrewing = false ;

    [Header("Referensi Buku Resep")]
    public RecipeBookData recipeBook;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryBrewPotion();
        }
    }

    public void TryBrewPotion()
    {
        if (cauldron.IsLocked()) return;
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
                EventManager.Raise(new BrewSuccessEvent());

                SpawnPotionObject(potionRecipe);
                cauldron.ClearAll();

                return;
            }
        }

        if (isBrewing == false)
        {
            Debug.Log("Tidak cocok dengan resep manapun.");
            isBrewing = true;
            dotBetweenTransform.DOShakePosition(
            duration: 0.3f,
            strength: new Vector3(0.3f, 0f, 0f),
            vibrato: 10,
            randomness: 0,
            snapping: false,
            fadeOut: true
        ).OnComplete(() =>
        {
            List<IngredientInstance> ingredients = cauldron.GetCurrentIngredients();
            int completed = 0;

            foreach (var ing in ingredients)
            {
                if (ing == null) continue;
                Collider2D col = ing.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                Vector3 randomOffset = new Vector3(
                    Random.Range(-1.5f, 1.5f),
                    Random.Range(-1f, -2.5f),
                    0f
                );

                ing.transform
                    .DOMove(ing.transform.position + randomOffset, 0.6f)
                    .SetEase(Ease.OutQuad);
                ing.transform
                    .DORotate(new Vector3(0, 0, Random.Range(180f, 720f)), 0.6f, RotateMode.FastBeyond360);
                DOVirtual.DelayedCall(0.6f, () =>
                {
                    Destroy(ing.gameObject);
                    completed++;

                    if (completed >= ingredients.Count)
                    {
                        cauldron.ClearAll();
                    }
                });
            }

            if (ingredients.Count == 0)
            {
                cauldron.ClearAll();
            }
            isBrewing = false;
        });
            
        }
        
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
