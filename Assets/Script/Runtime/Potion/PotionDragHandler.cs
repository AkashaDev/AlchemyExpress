using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    public Potion potionData;

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
        if (hit != null && hit.CompareTag("Customer"))
        {
            CustomerTestBehavior customer = hit.GetComponent<CustomerTestBehavior>();
            if (customer != null)
            {
                Debug.Log($"Potion {potionData.potionName} diberikan ke customer {hit.name}");
                customer.ReceivePotion(potionData);
            }

            Destroy(gameObject);
        }
    }

   private Vector3 GetMouseWorldPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
