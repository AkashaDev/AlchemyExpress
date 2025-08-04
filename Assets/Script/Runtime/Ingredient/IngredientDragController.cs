using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDragController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private CauldronSpawner cauldron;
    [SerializeField] private TrashAreaGrid trashArea;
    

    private IngredientInstance current;
    private Vector3 offset;
    private Vector2Int? lastGridPos = null;
    private bool isDragging = false;
    private Collider2D currentCollider;
    
    private string originalSortingLayer;
    private int originalSortingOrder;

    private bool _isFromInventory = false;
    private int _originSlotIndex = -1;

    // private void Awake()
    // {
    //     if (Instance != null && Instance != this) { Destroy(gameObject); }
    //     else { Instance = this; }
    // }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            HandleMouseDown();
        }

        if (isDragging)
        {
            HandleDragging();
            if (Input.GetMouseButtonDown(1)) Rotate(); // Gunakan Mouse1 untuk rotasi
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TryPlace();
        }
    }

     private void HandleMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);
        
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos2D);

        if (hits.Length == 0) return;
        
        foreach (var hit in hits)
        {
            WorldInventorySlot slot = hit.GetComponent<WorldInventorySlot>();
            if (slot != null)
            {
                IngredientSO itemToTake = InventoryManager.Instance.TakeItem(slot.slotIndex);
                if (itemToTake != null)
                {
                    StartDragFromInventory(itemToTake, mouseWorld, slot.slotIndex);
                }
                return;
            }
        }
        
        foreach (var hit in hits)
        {
            IngredientInstance inst = hit.GetComponent<IngredientInstance>();
            if (inst != null)
            {   
                inst.transform.localScale = Vector3.one;
                StartDragFromWorld(inst, mouseWorld);
                return;
            }
        }
    }

    private void StartDragFromWorld(IngredientInstance inst, Vector3 mouseWorld)
    {
        isDragging = true;
        _isFromInventory = false;
        _originSlotIndex = -1;

        current = inst;
        offset = inst.transform.position - mouseWorld;
        current.IsBeingDragged = true;

        currentCollider = current.GetComponent<Collider2D>();
        if (currentCollider != null) currentCollider.enabled = false;

        SpriteRenderer sr = current.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalSortingLayer = sr.sortingLayerName;
            originalSortingOrder = sr.sortingOrder;
            sr.sortingLayerName = "Gameplay";
            sr.sortingOrder = 100;
        }

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

    private void StartDragFromInventory(IngredientSO itemData, Vector3 mouseWorld, int originSlot)
    {
        isDragging = true;
        _isFromInventory = true;
        _originSlotIndex = originSlot;

        GameObject newObj = Instantiate(ingredientPrefab, mouseWorld, Quaternion.identity);
        current = newObj.GetComponent<IngredientInstance>();
        current.Setup(itemData);
        
        offset = Vector3.zero;
        current.IsBeingDragged = true;

        currentCollider = current.GetComponent<Collider2D>();
        if (currentCollider != null) currentCollider.enabled = false;
        
        SpriteRenderer sr = current.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalSortingLayer = sr.sortingLayerName;
            originalSortingOrder = sr.sortingOrder;
            sr.sortingLayerName = "Gameplay";
            sr.sortingOrder = 100;
        }

        cauldron.gridBehavior.ClearPreview();
        lastGridPos = null;
    }

    void HandleDragging()
    {
        if (cauldron == null || current == null)
            return;
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

        if (trashArea != null)
        {
            bool isHoveringTrash = trashArea.IsInsideTrashArea(mouseWorld);
            trashArea.SetHoverState(isHoveringTrash);
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
        current.IsBeingDragged = false;
        if (currentCollider != null) currentCollider.enabled = true;
        cauldron.gridBehavior.ClearPreview();

        // Kembalikan sorting layer
        SpriteRenderer sr = current.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = originalSortingLayer;
            sr.sortingOrder = originalSortingOrder;
        }
        
        if (trashArea != null) trashArea.SetHoverState(false);

        Collider2D[] hits = Physics2D.OverlapPointAll(current.transform.position);
        
        if (trashArea != null && trashArea.IsInsideTrashArea(current.transform.position))
        {
            Destroy(current.gameObject);
            return;
        }

        foreach (var hit in hits)
        {
            if (hit.CompareTag("InventoryDropZone"))
            {
                if (InventoryManager.Instance.AddItem(current.data))
                {
                    Destroy(current.gameObject);
                    return;
                }
            }
        }
        
        Vector2Int dropPos = cauldron.gridBehavior.WorldToGrid(current.transform.position);
        if (cauldron.gridBehavior.CanPlaceIngredient(current, dropPos))
        {
            current.transform.position = cauldron.gridBehavior.GridToWorld(dropPos);
            current.transform.SetParent(cauldron.tileParent);
            cauldron.gridBehavior.PlaceIngredient(current, dropPos);
            current.RememberPlacedInCauldron(current.transform.position, current.GetRotationIndex());
            OnPlacedInCauldronFeedback();
            return;
        }
        
        OnRejectedFeedback();
        if (_isFromInventory)
        {
            InventoryManager.Instance.ReturnItem(_originSlotIndex, current.data);
            Destroy(current.gameObject);
        }
        else
        {
            if (current.HasEverBeenPlaced)
            {
                current.ReturnToLastKnownValidPosition(cauldron);
                cauldron.gridBehavior.PlaceIngredient(current, cauldron.gridBehavior.WorldToGrid(current.transform.position));
            }
            else
            {   
                current.transform.localScale = Vector3.one*0.7f;
                current.ResetToSpawnPoint();
            }
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
