using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public IngredientSpawner spawner;
    public Transform conveyorStartPoint;
    public float spawnDelay = 2f;
    
    public List<IngridientData> ingredientsToSpawn = new List<IngridientData>();
    private int currentIndex = 0;

    void Start()
    {
        StartCoroutine(SpawnIngredientsRoutine());
    }

    private IEnumerator SpawnIngredientsRoutine()
    {
        while (currentIndex < ingredientsToSpawn.Count)
        {
            SpawnNewIngredient(ingredientsToSpawn[currentIndex]);
            currentIndex++;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void SpawnNewIngredient(IngridientData data) 
    {
        var ingredient = spawner.SpawnIngridient(data);
        ingredient.transform.position = conveyorStartPoint.position;
    }
}
