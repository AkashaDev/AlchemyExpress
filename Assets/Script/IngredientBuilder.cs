using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBuilder : MonoBehaviour
{
    public GameObject blockUnitPrefab; // Prefab 1 kotak (kotak biasa)
    public IngredientSO data;

    void Start()
    {
        if (data != null)
            BuildFromData(data);
    }

    public void BuildFromData(IngredientSO ingredient)
    {
        data = ingredient;
        ClearChildren();

        foreach (Vector2Int cell in ingredient.shapeCells)
        {
            CreateBlock(cell);
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && data.sprite != null)
            sr.sprite = data.sprite; // Opsional, bisa pakai sprite utama di parent
    }

    void CreateBlock(Vector2 cellPos)
    {
        GameObject go = Instantiate(blockUnitPrefab, transform);
        go.transform.localPosition = new Vector3(cellPos.x, cellPos.y, 0);

        if (go.GetComponent<ClickForwarder>() == null)
            go.AddComponent<ClickForwarder>();
    }

    void ClearChildren()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}