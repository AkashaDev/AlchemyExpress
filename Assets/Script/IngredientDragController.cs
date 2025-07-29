using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDragController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CauldronSpawner cauldron;

    private IngredientInstance current;
    private Vector3 offset;
    private Vector3 originalPos;

    private Vector2Int? lastValidGridPos = null;
    private int lastValidRotationIndex = 0;
    private Vector2Int? lastGridPos = null;

    private bool isDragging = false;
    private bool isPlacedInCauldron = false;

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
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

        if (hit != null)
        {
            IngredientInstance inst = hit.GetComponent<IngredientInstance>();
            if (inst != null)
            {
                current = inst;
                isDragging = true;
                originalPos = inst.transform.position;
                offset = inst.transform.position - mouseWorld;

                cauldron.gridBehavior.ClearPreview();

                if (isPlacedInCauldron)
                    cauldron.gridBehavior.RemoveIngredient(current);

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

            lastValidGridPos = dropPos;
            lastValidRotationIndex = current.GetRotationIndex();
            isPlacedInCauldron = true;

            OnPlacedInCauldronFeedback();
        }
        else
        {
            if (isPlacedInCauldron && lastValidGridPos.HasValue)
            {
                current.transform.position = cauldron.gridBehavior.GridToWorld(lastValidGridPos.Value);
                current.SetRotationIndex(lastValidRotationIndex);
                current.transform.SetParent(cauldron.tileParent);
                cauldron.gridBehavior.PlaceIngredient(current, lastValidGridPos.Value);
            }
            else
            {
                current.transform.SetParent(null);
                current.transform.position = originalPos;
            }

            OnRejectedFeedback();
        }

        cauldron.gridBehavior.ClearPreview();
        current = null;
        lastGridPos = null;
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
