using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDragController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CauldronSpawner cauldron;

    private IngredientInstance current;
    private Vector3 offset;
    private Vector2Int? lastGridPos = null;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
            TryPickIngredient();

        if (isDragging)
        {
            HandleDragging();

            if (Input.GetKeyDown(KeyCode.R))
                Rotate();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
            TryPlace();
    }

    void TryPickIngredient()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos2D);

        if (hits.Length == 0)
            return;

        foreach (Collider2D hit in hits)
        {
            IngredientInstance inst = hit.GetComponent<IngredientInstance>();
            if (inst != null)
            {
                current = inst;
                isDragging = true;
                offset = inst.transform.position - mouseWorld;

                if (cauldron.gridBehavior.IsIngredientPlaced(current))
                {
                    cauldron.gridBehavior.RemoveIngredient(current);
                }
                else
                {
                current.RememberSpawnPosition();
                }

                cauldron.gridBehavior.ClearPreview();
                lastGridPos = null;
            }
        }
    }

    void HandleDragging()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
        mouseWorld.z = 0;
        current.transform.position = mouseWorld;

        Vector2Int gridPos = cauldron.gridBehavior.WorldToGrid(mouseWorld);

        if (lastGridPos != gridPos)
        {
            cauldron.gridBehavior.ClearPreview();
            bool canPlace = cauldron.gridBehavior.CanPlaceIngredient(current, gridPos);
            Vector2Int[] shape = current.GetRotatedShapeForExternal();
            cauldron.gridBehavior.PreviewPlacement(shape, gridPos, canPlace);
            lastGridPos = gridPos;
        }
    }

    void Rotate()
    {
        current.RotateClockwise();
        Vector2Int gridPos = cauldron.gridBehavior.WorldToGrid(current.transform.position);
        cauldron.gridBehavior.ClearPreview();

        bool canPlace = cauldron.gridBehavior.CanPlaceIngredient(current, gridPos);
        Vector2Int[] shape = current.GetRotatedShapeForExternal();
        cauldron.gridBehavior.PreviewPlacement(shape, gridPos, canPlace);
    }

    void TryPlace()
    {
        isDragging = false;

        Vector2Int dropPos = cauldron.gridBehavior.WorldToGrid(current.transform.position);

        if (cauldron.gridBehavior.CanPlaceIngredient(current, dropPos))
        {
            current.transform.position = cauldron.gridBehavior.GridToWorld(dropPos);
            current.transform.SetParent(cauldron.tileParent);
            cauldron.gridBehavior.PlaceIngredient(current, dropPos);

            current.RememberPlacedInCauldron(current.transform.position, current.GetRotationIndex());

            OnPlacedInCauldronFeedback();
        }
        else
        {
            if (current.HasEverBeenPlaced)
            {
                current.ReturnToLastKnownValidPosition(cauldron);

                if (cauldron.gridBehavior.CanPlaceIngredient(current, cauldron.gridBehavior.WorldToGrid(current.transform.position)))
                {
                    cauldron.gridBehavior.PlaceIngredient(current, cauldron.gridBehavior.WorldToGrid(current.transform.position));
                    Debug.Log("Fallback placement berhasil.");
                }
                else
                {
                    Debug.LogError("Fallback placement gagal, grid sudah terisi/tidak valid.");
                    current.ResetToSpawnPoint();
                }
            }
            else
            {
                current.ResetToSpawnPoint();
            }

            OnRejectedFeedback();
        }
    }

    void OnPlacedInCauldronFeedback()
    {
        Debug.Log("Ingredient berhasil ditempatkan di cauldron!");
    }

    void OnRejectedFeedback()
    {
        Debug.Log("Ingredient ditolak, kembali ke conveyor.");
    }
}
