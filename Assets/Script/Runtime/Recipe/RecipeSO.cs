using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe" , menuName = "Potion/Recipe")]
public class RecipeSO : ScriptableObject
{
    public string recipeName;
    public List<string> requiredIngredientsName;
    public PotionSO resultPotion;
}
