// using UnityEngine;
// using UnityEngine.EventSystems;

// public class InventorySlotUI : MonoBehaviour, IPointerDownHandler
// {
//     // Atur indeks ini di Inspector untuk setiap slot (0, 1, 2, ...)
//     public int slotIndex; 
    
//     // Dipanggil saat slot ini di-klik
//     public void OnPointerDown(PointerEventData eventData)
//     {
//         // Coba ambil item dari manager
//         IngredientSO itemToTake = InventoryManager.Instance.TakeItem(slotIndex);
        
//         // Jika berhasil (slot tidak kosong)
//         if (itemToTake != null)
//         {
//             // Suruh drag controller untuk memulai drag dengan item ini
//             IngredientDragController.Instance.StartDragFromInventory(itemToTake);
//         }
//     }
// }