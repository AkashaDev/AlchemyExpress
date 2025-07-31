using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    [Tooltip("Kecepatan dasar gerak item. Gunakan nilai negatif untuk bergerak ke kiri.")]
    [SerializeField] private float speed = 2.0f;

    [Tooltip("Jarak 'gap' KOSONG minimal antar tepi item.")]
    [SerializeField] private float minimumItemGap = 0.1f;

    [Header("Pengaturan Visual")]
    [Tooltip("Faktor pengali untuk kecepatan scroll tekstur.")]
    [SerializeField] private float visualSpeedFactor = 0.25f;

    [Header("Referensi Komponen")]
    public Transform spawnPoint;
    
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
        float direction = Mathf.Sign(speed);
        if (direction == 0) return;

        Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _beltCollider.offset,
            _beltCollider.size,
            transform.eulerAngles.z
        );

        // ✨ PERBAIKAN KUNCI 1: Ubah List menjadi <Collider2D> untuk mengakses .bounds
        List<Collider2D> itemCollidersOnBelt = overlappingColliders
            .Where(col => col.GetComponent<IngredientInstance>() != null)
            .ToList();
        
        if (itemCollidersOnBelt.Count == 0) return;

        // ✨ PERBAIKAN KUNCI 2: Urutkan berdasarkan PUSAT COLLIDER, bukan posisi transform
        itemCollidersOnBelt = (direction > 0)
            ? itemCollidersOnBelt.OrderByDescending(col => col.bounds.center.x).ToList()
            : itemCollidersOnBelt.OrderBy(col => col.bounds.center.x).ToList();

        float beltEdgeX = transform.position.x + _beltCollider.offset.x + (direction * _beltCollider.size.x / 2);

        for (int i = itemCollidersOnBelt.Count - 1; i >= 0; i--)
        {
            Collider2D currentItemCollider = itemCollidersOnBelt[i];
            if (currentItemCollider == null) continue;

            Collider2D itemInFrontCollider = (i == 0) ? null : itemCollidersOnBelt[i - 1];
            
            MoveItemWithSpacing(currentItemCollider, itemInFrontCollider, beltEdgeX, direction);
        }
    }

    private void MoveItemWithSpacing(Collider2D currentItemCollider, Collider2D itemInFrontCollider, float beltEdgeX, float direction)
    {
        Rigidbody2D currentItemRb = currentItemCollider.attachedRigidbody;
        if (currentItemRb == null) return;

        float currentItemHalfWidth = currentItemCollider.bounds.size.x / 2f;
        float edgeStopPosition = beltEdgeX - (direction * currentItemHalfWidth);
        float finalStopPosition = edgeStopPosition;

        if (itemInFrontCollider != null)
        {
            float itemInFrontHalfWidth = itemInFrontCollider.bounds.size.x / 2f;
            float frontItemCenterX = itemInFrontCollider.bounds.center.x;
            
            float spacingLimit = frontItemCenterX - (direction * (itemInFrontHalfWidth + currentItemHalfWidth + minimumItemGap));

            finalStopPosition = (direction > 0)
                ? Mathf.Min(edgeStopPosition, spacingLimit)
                : Mathf.Max(edgeStopPosition, spacingLimit);
        }

        // Hitung posisi baru berdasarkan pivot saat ini
        Vector2 potentialNewPosition = currentItemRb.position + (Vector2.right * speed * Time.fixedDeltaTime);
        
        // ✨ PERBAIKAN KUNCI 3: Kompensasi perbedaan antara pivot dan pusat bounds
        float pivotToCenterOffsetX = currentItemCollider.bounds.center.x - currentItemRb.position.x;
        float correctedFinalStopPosition = finalStopPosition - pivotToCenterOffsetX;

        potentialNewPosition.x = (direction > 0)
            ? Mathf.Min(potentialNewPosition.x, correctedFinalStopPosition)
            : Mathf.Max(potentialNewPosition.x, correctedFinalStopPosition);
            
        currentItemRb.MovePosition(potentialNewPosition);
    }
}