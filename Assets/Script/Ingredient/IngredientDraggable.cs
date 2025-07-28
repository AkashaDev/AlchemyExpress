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
    private Transform lastTileTransform = null;
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

        // Putus parent saat mulai drag
        transform.SetParent(null);
        cauldron?.ClearPreview();
        lastPreviewPos = null;

        if (isPlacedInCauldron)
        {
            cauldron?.UnregisterIngredient(instance);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        Vector2 pivotWorldPos = instance.GetPivotWorldPosition();
Vector2Int dropPos = new Vector2Int(Mathf.RoundToInt(pivotWorldPos.x), Mathf.RoundToInt(pivotWorldPos.y));

        if (cauldron != null && cauldron.CanAcceptIngredient(instance, dropPos))
        {
            // Dapatkan tile tempat ingredient akan masuk
            Transform tileTransform = cauldron.GetTileTransformAt(dropPos);

            if (tileTransform != null)
            {
                transform.SetParent(tileTransform);
                transform.localPosition = Vector3.zero;
                lastTileTransform = tileTransform;
            }

            cauldron.RegisterIngredient(instance, dropPos);
            lastValidGridPos = dropPos;
            isPlacedInCauldron = true;
            lastValidRotationIndex = instance.GetRotationIndex();

            OnPlacedInCauldronFeedback();
        }
        else
        {
            if (isPlacedInCauldron && lastValidGridPos.HasValue && lastTileTransform != null)
            {
                // Kembali ke posisi valid terakhir
                transform.SetParent(lastTileTransform);
                transform.localPosition = Vector3.zero;
                instance.SetRotationIndex(lastValidRotationIndex);
            }
            else
            {
                // Kembali ke conveyor
                transform.SetParent(null);
                transform.position = originalPosition;
            }

            OnRejectedFeedback();
        }
        cauldron?.ClearPreview();
        lastPreviewPos = null;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
            mousePos.z = 0;

            transform.position = mousePos;

            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

            if (lastPreviewPos != gridPos)
            {
                cauldron?.ClearPreview();

                if (cauldron != null)
                {
                    Vector2Int[] shape = instance.GetRotatedShapeForExternal();
                    bool valid = cauldron.CanAcceptIngredient(instance, gridPos);
                    cauldron.PreviewPlacement(shape, gridPos, valid);
                }

                lastPreviewPos = gridPos;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                instance.RotateClockwise();
            }
        }
    }


    private void OnPlacedInCauldronFeedback()
    {
        Debug.Log("Ingredient berhasil ditempatkan di cauldron!");
        // Tambahkan efek visual/suara/warna jika diinginkan
    }

    private void OnInvalidMoveFeedback()
    {
        Debug.Log("Posisi tidak valid, kembali ke tempat sebelumnya.");
        // Tambahkan efek getaran atau highlight merah
    }

    private void OnRejectedFeedback()
    {
        Debug.Log("Ingredient ditolak, kembali ke conveyor.");
        // Tambahkan efek seperti scale down, suara tolak, animasi bounce
    }
}
