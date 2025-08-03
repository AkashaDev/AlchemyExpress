using ObeserverPattern;
using UnityEngine;

public class PotionDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    public Potion potionData;

    private Vector3 _spawnPosition;

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
        _spawnPosition = transform.position;
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        bool droppedOnCustomer = false;
        bool droppedInTrash = false;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Customer"))
            {
                droppedOnCustomer = true;
            }
            if (hit.CompareTag("Trashbin"))
            {
                droppedInTrash = true;
            }
        }

        if (droppedOnCustomer)
        {
            EventManager.Raise(new PotionDisposedEvent());
            EventManager.Raise(new PotionGivenToNPCEvent { potion = this.potionData });
            Destroy(gameObject);
        }
        else if (droppedInTrash)
        {
            EventManager.Raise(new PotionDisposedEvent());
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
