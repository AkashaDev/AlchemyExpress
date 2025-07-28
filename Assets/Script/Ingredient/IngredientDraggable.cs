using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDraggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;
    private IngredientInstance instance;
    /*private int currentRotation = 0;*/

    void Start()
    {
        cam = Camera.main;
        instance = GetComponent<IngredientInstance>();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0) + offset;
        }

        if (isDragging && instance.data.canRotate && Input.GetKeyDown(KeyCode.R))
        {
            instance.RotateClockwise(); // Pakai grid-based rotate, bukan transform.Rotate
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        isDragging = true;
        offset = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Di sinilah kamu bisa cek apakah bahan masuk ke cauldron, trash, dll.
        // Kalau tidak di tempat valid, bisa reset posisi atau destroy
    }
}
