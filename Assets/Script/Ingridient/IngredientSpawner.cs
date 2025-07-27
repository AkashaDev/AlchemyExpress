using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public Transform spawnParent;
    public GameObject tilePrefab;

    public IngredientObject SpawnIngridient(IngridientData data) 
    {
        GameObject go = new GameObject(data.ingridientName);
        go.transform.SetParent(spawnParent);
        var ingridient = go.AddComponent<IngredientObject>();
        ingridient.Init(data,tilePrefab);
        var dragHandler = go.AddComponent<IngredientDragHandler>();
/*        go.AddComponent<BoxCollider2D>();*/
        return ingridient;
    }
}
