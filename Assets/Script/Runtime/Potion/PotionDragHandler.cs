using UnityEngine;
using ObeserverPattern;

public class PotionDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    public Potion potionData;

    private Vector3 _spawnPosition;
    private TrashAreaGrid _trashArea;


    private void Update()
    {
        if (isDragging)
        {
            Vector3 pos = GetMouseWorldPos() + offset;
            pos.z = 0;
            transform.position = pos;
        }
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        Collider2D hit = Physics2D.OverlapPoint(transform.position);

        bool droppedOnCustomer = hit != null && hit.CompareTag("Customer");
        bool droppedInTrash = _trashArea != null && _trashArea.IsInsideTrashArea(transform.position);

        if (droppedOnCustomer || droppedInTrash)
        {
            EventManager.Raise(new PotionDisposedEvent());

            if(droppedOnCustomer) EventManager.Raise(new PotionGivenToNPCEvent { potion = this.potionData });

            Destroy(gameObject);
        }
        else
        {
            transform.position = _spawnPosition;
        }
    }

   private Vector3 GetMouseWorldPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
