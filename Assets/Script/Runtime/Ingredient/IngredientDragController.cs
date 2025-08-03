using ObeserverPattern;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDragController : MonoBehaviour
{
    public static IngredientDragController Instance { get; private set; }

    [Header("Referensi")]
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private GameObject ingredientPrefab;

    [SerializeField]
    private CauldronSpawner cauldron;

    [SerializeField]
    private TrashAreaGrid trashArea;

    private IngredientInstance current;
    private Vector3 offset;
    private Vector2Int? lastGridPos = null;
    private bool isDragging = false;
    private Collider2D currentCollider;
    private Vector3 _returnPosition;
    private bool _isFromInventory = false;
    private int _originSlotIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            HandleMouseDown();
        }

        if (isDragging)
        {
            HandleDragging();
            if (Input.GetMouseButtonDown(1))
                Rotate();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TryPlace();
        }
    }

    private void HandleMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 mouseWorld = GetMouseWorldPos();
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos2D);

        if (hits.Length == 0)
            return;

        // Loop pertama untuk prioritas slot inventaris
        foreach (var hit in hits)
        {
            WorldInventorySlot slot = hit.GetComponent<WorldInventorySlot>();
            if (slot != null)
            {
                IngredientSO itemToTake = InventoryManager.Instance.TakeItem(slot.slotIndex);
                if (itemToTake != null)
                {
                    StartDrag(
                        itemToTake,
                        mouseWorld,
                        true,
                        slot.transform.position,
                        slot.slotIndex
                    );
                }
                return;
            }
        }

        foreach (var hit in hits)
        {
            IngredientInstance inst = hit.GetComponent<IngredientInstance>();
            if (inst != null)
            {
                StartDrag(inst, mouseWorld, false);
                return;
            }
        }
    }

    private void StartDrag(
        object item,
        Vector3 mouseWorld,
        bool fromInventory,
        Vector3? returnPos = null,
        int originSlot = -1
    )
    {
        isDragging = true;
        _isFromInventory = fromInventory;
        _originSlotIndex = originSlot;

        if (fromInventory)
        {
            GameObject newObj = Instantiate(ingredientPrefab, mouseWorld, Quaternion.identity);
            current = newObj.GetComponent<IngredientInstance>();
            current.Setup(item as IngredientSO);
            offset = Vector3.zero;
            _returnPosition = returnPos.Value;
        }
        else
        {
            current = item as IngredientInstance;
            offset = current.transform.position - mouseWorld;
            _returnPosition = current.transform.position;
        }

        current.IsBeingDragged = true;
        currentCollider = current.GetComponent<Collider2D>();
        if (currentCollider != null)
            currentCollider.enabled = false;

        if (cauldron.gridBehavior.IsIngredientPlaced(current))
        {
            cauldron.gridBehavior.RemoveIngredient(current);
        }

        cauldron.gridBehavior.ClearPreview();
        lastGridPos = null;
    }

    private void HandleDragging()
    {
        Vector3 mouseWorld = GetMouseWorldPos() + offset;
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

    private void TryPlace()
    {
        isDragging = false;
        current.IsBeingDragged = false;
        if (currentCollider != null)
            currentCollider.enabled = true;
        cauldron.gridBehavior.ClearPreview();

        Collider2D[] hits = Physics2D.OverlapPointAll(current.transform.position);
        bool droppedOnValidZone = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Trashbin"))
            {
                Debug.Log($"Bahan {current.data.ingredientName} dibuang.");
                Destroy(current.gameObject);
                droppedOnValidZone = true;
                break;
            }

            if (hit.CompareTag("InventoryDropZone"))
            {
                if (InventoryManager.Instance.AddItem(current.data))
                {
                    Debug.Log($"Bahan {current.data.ingredientName} dimasukkan ke inventaris.");
                    Destroy(current.gameObject);
                    droppedOnValidZone = true;
                }
                break;
            }
        }

        if (droppedOnValidZone)
            return;

        Vector2Int dropPos = cauldron.gridBehavior.WorldToGrid(current.transform.position);
        if (cauldron.gridBehavior.CanPlaceIngredient(current, dropPos))
        {
            current.transform.position = cauldron.gridBehavior.GridToWorld(dropPos);
            current.transform.SetParent(cauldron.tileParent);
            cauldron.gridBehavior.PlaceIngredient(current, dropPos);
            current.RememberPlacedInCauldron(
                current.transform.position,
                current.GetRotationIndex()
            );
            return;
        }

        Debug.Log("Drop tidak valid, mengembalikan item.");
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
                cauldron.gridBehavior.PlaceIngredient(
                    current,
                    cauldron.gridBehavior.WorldToGrid(current.transform.position)
                );
            }
            else
            {
                current.transform.position = _returnPosition;
            }
        }
    }

    private void Rotate()
    {
        current.RotateClockwise();
        HandleDragging();
    }

    private Vector3 GetMouseWorldPos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }
}
