using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public IngredientSO[] allIngredients;
    public GameObject baseIngredientPrefab; // prefab "IngredientBase" (kosong + builder)

    public Transform spawnPoint;

    private void Start()
    {
        SpawnRandomIngredient();
    }

    public void SpawnRandomIngredient()
    {
        int index = Random.Range(0, allIngredients.Length);
        IngredientSO chosen = allIngredients[index];

        GameObject obj = Instantiate(baseIngredientPrefab, spawnPoint.position, Quaternion.identity);
        var builder = obj.GetComponent<IngredientBuilder>();
        var block = obj.GetComponent<IngredientBlock>();

        builder.BuildFromData(chosen);
        block.data = chosen;
    }
}