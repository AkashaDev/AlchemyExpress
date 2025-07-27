using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isDragging = false;

    private CauldronArea cauldronArea;

    void Start()
    {
        cauldronArea = FindObjectOfType<CauldronArea>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        originalPosition = transform.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0);
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0) + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (cauldronArea != null && cauldronArea.IsInside(transform.position))
        {
            transform.position = cauldronArea.SnapToGrid(transform.position);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 0, 90f);
        }
    }
}
