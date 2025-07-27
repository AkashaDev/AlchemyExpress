using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBlock : MonoBehaviour
{
    private Vector3 originalPosition;
    private Camera cam;
    private bool isDragging = false;

    public IngredientSO data;

    void Start()
    {
        originalPosition = transform.position;
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.position = SnapToGrid(transform.position);
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }

        if (isDragging && data != null && data.canRotate && Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 0, 90f);
        }
    }

    Vector2 SnapToGrid(Vector2 pos)
    {
        float size = 1f;
        float x = Mathf.Round(pos.x / size) * size;
        float y = Mathf.Round(pos.y / size) * size;
        return new Vector2(x, y);
    }

    public void ResetPosition()
    {
        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
    }
}
