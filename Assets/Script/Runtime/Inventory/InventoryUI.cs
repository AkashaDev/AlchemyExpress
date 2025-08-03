using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ObeserverPattern;

public class InventoryUI : MonoBehaviour
{
    // Drag semua GameObject slot UI Anda ke list ini di Inspector
    [SerializeField] private List<Image> slotIcons; 

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
        // Gambar tampilan awal saat game mulai
        Redraw(new InventoryChangedEvent());
    }

    /// <summary>
    /// Menggambar ulang semua ikon di inventaris berdasarkan data dari InventoryManager.
    /// </summary>
    private void Redraw(InventoryChangedEvent e)
    {
        List<IngredientSO> currentItems = InventoryManager.Instance.GetCurrentItems();

        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (i < currentItems.Count && currentItems[i] != null)
            {
                // Jika slot terisi, tampilkan ikonnya
                slotIcons[i].sprite = currentItems[i].itemIcon;
                slotIcons[i].enabled = true;
            }
            else
            {
                // Jika slot kosong, sembunyikan ikonnya
                slotIcons[i].sprite = null;
                slotIcons[i].enabled = false;
            }
        }
    }
}