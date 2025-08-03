using UnityEngine;
using System.Collections.Generic;
using ObeserverPattern;

public class WorldInventoryDisplay : MonoBehaviour
{
    [Header("Referensi Slot")]
    [Tooltip("Drag semua GameObject Slot INDUK Anda ke sini (misal: Slot_0, Slot_1).")]
    [SerializeField] private List<GameObject> slotObjects;

    private void OnEnable()
    {
        EventManager.Subscribe<InventoryChangedEvent>(Redraw);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<InventoryChangedEvent>(Redraw);
    }

    private void Start()
    {
        Redraw(new InventoryChangedEvent());
    }

    private void Redraw(InventoryChangedEvent e)
    {
        if (InventoryManager.Instance == null) return;
        List<IngredientSO> currentItems = InventoryManager.Instance.GetCurrentItems();

        for (int i = 0; i < slotObjects.Count; i++)
        {
            if (slotObjects[i] == null) continue;

            SpriteRenderer iconRenderer = slotObjects[i].GetComponentInChildren<SpriteRenderer>();
            BoxCollider2D slotCollider = slotObjects[i].GetComponent<BoxCollider2D>();

            if (iconRenderer == null || slotCollider == null)
            {
                Debug.LogWarning($"Slot #{i} tidak memiliki setup yang benar! Pastikan ada BoxCollider2D di induk dan SpriteRenderer di anak.", slotObjects[i]);
                continue;
            }

            bool hasItem = i < currentItems.Count && currentItems[i] != null;
            iconRenderer.enabled = hasItem;

            if (hasItem)
            {
                iconRenderer.sprite = currentItems[i].itemIcon;

            
                Bounds spriteBounds = iconRenderer.sprite.bounds;
                if (spriteBounds.size.x == 0 || spriteBounds.size.y == 0) continue;

                Vector2 targetSize = slotCollider.size;
                
                float scaleX = targetSize.x / spriteBounds.size.x;
                float scaleY = targetSize.y / spriteBounds.size.y;
                
                float finalScale = Mathf.Min(scaleX, scaleY);
                
                iconRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
                
                iconRenderer.transform.localPosition = Vector3.zero;
            }
        }
    }
}