using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDraggable : MonoBehaviour
{
    private Camera cam;
    private IngredientInstance instance;
    private CauldronSpawner cauldron;

    private Vector3 offset;
    private Vector3 originalPosition;
    private Vector2Int? lastValidGridPos = null;
    private Vector2Int? lastPreviewPos = null;
    private int lastValidRotationIndex = 0;

    private bool isDragging = false;
    private bool isPlacedInCauldron = false;

    void Start()
    {
        cam = Camera.main;
        instance = GetComponent<IngredientInstance>();
        cauldron = FindObjectOfType<CauldronSpawner>();
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        isDragging = true;
        offset = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        transform.SetParent(null);
        cauldron?.gridBehavior.ClearPreview();
        lastPreviewPos = null;

        if (isPlacedInCauldron)
        {
            cauldron?.gridBehavior.RemoveIngredient(instance);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        Vector2 pivotWorldPos = instance.GetPivotWorldPosition();
        Vector2Int dropPos = cauldron.gridBehavior.WorldToGrid(pivotWorldPos);

        if (cauldron != null && cauldron.gridBehavior.CanPlaceIngredient(instance, dropPos))
        {
            // Tempatkan ingredient langsung di world space
            transform.position = cauldron.gridBehavior.GridToWorld(dropPos);
            transform.SetParent(cauldron.tileParent); // optional parenting

            cauldron.gridBehavior.PlaceIngredient(instance, dropPos);

            lastValidGridPos = dropPos;
            isPlacedInCauldron = true;
            lastValidRotationIndex = instance.GetRotationIndex();

            OnPlacedInCauldronFeedback();
        }
        else
        {
            if (isPlacedInCauldron && lastValidGridPos.HasValue)
            {
                transform.position = cauldron.gridBehavior.GridToWorld(lastValidGridPos.Value);
                instance.SetRotationIndex(lastValidRotationIndex);
                transform.SetParent(cauldron.tileParent);
                cauldron.gridBehavior.PlaceIngredient(instance, lastValidGridPos.Value);
            }
            else
            {
                transform.SetParent(null);
                transform.position = originalPosition;
            }

            OnRejectedFeedback();
        }

        cauldron?.gridBehavior.ClearPreview();
        lastPreviewPos = null;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
            mousePos.z = 0;
            transform.position = mousePos;

            Vector2Int gridPos = cauldron.gridBehavior.WorldToGrid(mousePos);

            if (lastPreviewPos != gridPos)
            {
                cauldron?.gridBehavior.ClearPreview();

                if (cauldron != null)
                {
                    Vector2Int[] shape = instance.GetRotatedShapeForExternal();
                    bool valid = cauldron.gridBehavior.CanPlaceIngredient(instance, gridPos);
                    cauldron.gridBehavior.PreviewPlacement(shape, gridPos, valid);
                }

                lastPreviewPos = gridPos;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                instance.RotateClockwise();
                Vector2Int newGridPos = cauldron.gridBehavior.WorldToGrid(transform.position);
                cauldron.gridBehavior.ClearPreview();

                Vector2Int[] newShape = instance.GetRotatedShapeForExternal();
                bool valid = cauldron.gridBehavior.CanPlaceIngredient(instance, newGridPos);
                cauldron.gridBehavior.PreviewPlacement(newShape, newGridPos, valid);
            }
        }
    }

    private void OnPlacedInCauldronFeedback()
    {
        Debug.Log("Ingredient berhasil ditempatkan di cauldron!");
    }

    private void OnRejectedFeedback()
    {
        Debug.Log("Ingredient ditolak, kembali ke conveyor.");
    }
}
