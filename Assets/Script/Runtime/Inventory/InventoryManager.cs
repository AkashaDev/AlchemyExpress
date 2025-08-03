using UnityEngine;
using System.Collections.Generic;
using ObeserverPattern;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int inventorySize = 8;
    private List<IngredientSO> items;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        items = new List<IngredientSO>();
        for (int i = 0; i < inventorySize; i++) items.Add(null);
    }

    public bool AddItem(IngredientSO itemData)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemData;
                EventManager.Raise(new InventoryChangedEvent());
                return true;
            }
        }
        return false;
    }

    public IngredientSO TakeItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count || items[slotIndex] == null) return null;
        IngredientSO itemData = items[slotIndex];
        items[slotIndex] = null;
        EventManager.Raise(new InventoryChangedEvent());
        return itemData;
    }

    public void ReturnItem(int slotIndex, IngredientSO itemData)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;
        items[slotIndex] = itemData;
        EventManager.Raise(new InventoryChangedEvent());
    }

    public List<IngredientSO> GetCurrentItems() => items;
}