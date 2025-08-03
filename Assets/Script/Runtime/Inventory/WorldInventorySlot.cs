using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldInventorySlot : MonoBehaviour
{
    // Atur di Inspector (0, 1, 2, ...) untuk setiap slot
    public int slotIndex;
}