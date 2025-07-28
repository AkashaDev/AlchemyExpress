using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    // ✨ DIUBAH: 'itemSpacing' sekarang menjadi 'minimumItemGap' untuk lebih jelas
    [Tooltip("Jarak 'gap' KOSONG minimal antar tepi item.")]
    [SerializeField] private float minimumItemGap = 0.1f;

    [Tooltip("Offset dari ujung conveyor untuk titik berhenti. Biasanya setengah dari lebar item.")]
    [SerializeField] private float endPointOffset = 0.5f;

    [Header("Pengaturan Visual")]
    [Tooltip("Faktor pengali untuk kecepatan scroll tekstur. Gunakan nilai negatif untuk membalik arah visual jika perlu.")]
    [SerializeField] private float visualSpeedFactor = 0.25f;

    [Header("Referensi Komponen")]
    public Transform spawnPoint;

    private float speed;
    private BoxCollider2D _beltCollider;
    private Renderer _renderer;

    public void Initialize(float newSpeed)
    {
        this.speed = newSpeed;
    }

    private void Awake()
    {
        _beltCollider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<Renderer>();
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void Update()
    {
        if (_renderer?.material == null) return;
        float scrollOffset = speed * visualSpeedFactor * Time.deltaTime;
        _renderer.material.mainTextureOffset += new Vector2(scrollOffset, 0);
    }

    private void FixedUpdate()
    {
        // Bagian deteksi dan pengurutan tidak berubah
        float direction = Mathf.Sign(speed);
        if (direction == 0) return;

        Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(
            _beltCollider.bounds.center,
            _beltCollider.bounds.size,
            0f
        );

        List<Rigidbody2D> itemsOnBelt = overlappingColliders
            .Select(col => col.GetComponent<Rigidbody2D>())
            .Where(rb => rb != null && rb.GetComponent<IngredientInstance>() != null)
            .ToList();

        if (itemsOnBelt.Count == 0) return;

        itemsOnBelt = (direction > 0)
            ? itemsOnBelt.OrderByDescending(rb => rb.transform.position.x).ToList()
            : itemsOnBelt.OrderBy(rb => rb.transform.position.x).ToList();

        float beltEdgeX = transform.position.x + _beltCollider.offset.x + (direction * _beltCollider.size.x / 2);
        float stopPositionX = beltEdgeX - (direction * endPointOffset);

        for (int i = 0; i < itemsOnBelt.Count; i++)
        {
            Rigidbody2D currentItem = itemsOnBelt[i];
            Rigidbody2D itemInFront = (i == 0) ? null : itemsOnBelt[i - 1];
            
            // Panggil fungsi yang telah diperbarui
            MoveItemWithDynamicSpacing(currentItem, itemInFront, stopPositionX, direction);
        }
    }

    // ✨ FUNGSI INTI YANG DIPERBARUI ✨
    private void MoveItemWithDynamicSpacing(Rigidbody2D currentItem, Rigidbody2D itemInFront, float stopPositionX, float direction)
    {
        Vector2 potentialNewPosition = currentItem.position + (Vector2.right * speed * Time.fixedDeltaTime);
        float maxAllowedPosition = stopPositionX;

        if (itemInFront != null)
        {
            // Ambil komponen IngredientInstance dari kedua item
            IngredientInstance currentIngredient = currentItem.GetComponent<IngredientInstance>();
            IngredientInstance frontIngredient = itemInFront.GetComponent<IngredientInstance>();

            if (currentIngredient != null && frontIngredient != null)
            {
                // Hitung lebar masing-masing item berdasarkan shapeCells mereka
                float currentItemHalfWidth = GetIngredientWidth(currentIngredient) / 2f;
                float frontItemHalfWidth = GetIngredientWidth(frontIngredient) / 2f;

                // Hitung batas jarak yang dinamis
                float spacingLimit = itemInFront.position.x - 
                                     (direction * (frontItemHalfWidth + currentItemHalfWidth + minimumItemGap));
                
                maxAllowedPosition = (direction > 0)
                    ? Mathf.Min(maxAllowedPosition, spacingLimit)
                    : Mathf.Max(maxAllowedPosition, spacingLimit);
            }
        }

        potentialNewPosition.x = (direction > 0)
            ? Mathf.Min(potentialNewPosition.x, maxAllowedPosition)
            : Mathf.Max(potentialNewPosition.x, maxAllowedPosition);

        currentItem.MovePosition(potentialNewPosition);
    }

    private float GetIngredientWidth(IngredientInstance ingredient)
    {
        Vector2Int[] shape = ingredient.GetRotatedShapeForExternal();
        if (shape == null || shape.Length == 0) return 1f; // Default width

        int minX = shape[0].x;
        int maxX = shape[0].x;
        foreach (var cell in shape)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.x > maxX) maxX = cell.x;
        }
        
        return (maxX - minX) + 1;
    }
    
    private void OnDrawGizmosSelected()
    {
        BoxCollider2D beltCollider = GetComponent<BoxCollider2D>();
        if (beltCollider == null) return;

        float currentSpeed = Application.isPlaying ? this.speed : 2.0f;
        float direction = Mathf.Sign(currentSpeed);

        Vector2 boxCenter = (Vector2)transform.position + beltCollider.offset;
        Vector2 boxSize = beltCollider.size;
        
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.5f); 
        Gizmos.DrawCube(boxCenter, boxSize);

        float beltEdgeX = transform.position.x + beltCollider.offset.x + (direction * beltCollider.size.x / 2);
        float stopPositionX = beltEdgeX - (direction * endPointOffset);
        
        float topY = beltCollider.bounds.center.y + beltCollider.bounds.extents.y;
        float bottomY = beltCollider.bounds.center.y - beltCollider.bounds.extents.y;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(stopPositionX, bottomY, 0), new Vector3(stopPositionX, topY, 0));
    }
}