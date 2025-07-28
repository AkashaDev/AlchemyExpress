using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [Header("Ingredient Settings")]
    public IngredientSO[] ingredientPool;
    public GameObject ingredientPrefab;
    public Transform spawnPoint;

    [Header("Timing")]
    public float spawnInterval = 2f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnRandomIngredient();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomIngredient()
    {
        IngredientSO selected = ingredientPool[Random.Range(0, ingredientPool.Length)];
        GameObject obj = Instantiate(ingredientPrefab, spawnPoint.position, Quaternion.identity);
        IngredientInstance instance = obj.GetComponent<IngredientInstance>();
        instance.Setup(selected);
    }
}
