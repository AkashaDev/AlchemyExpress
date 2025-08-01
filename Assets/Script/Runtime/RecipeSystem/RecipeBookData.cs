using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Recipe Book Data", menuName = "Recipe System/Recipe Book")]
public class RecipeBookData : ScriptableObject
{
    [Header("All Recipes")]
    public List<Potion> allPotions = new List<Potion>();
    
    public List<Potion> GetAllPotions()
    {
        return allPotions;
    }
}