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

        List<Rigidbody2D> itemsOnBelt = overlappingColliders
            .Select(col => col.GetComponent<Rigidbody2D>())
            .Where(rb => rb != null && rb.GetComponent<IngredientInstance>() != null)
            .ToList();
        
        if (itemsOnBelt.Count == 0) return;

        itemsOnBelt = (direction > 0)
            ? itemsOnBelt.OrderByDescending(rb => rb.transform.position.x).ToList()
            : itemsOnBelt.OrderBy(rb => rb.transform.position.x).ToList();

        float beltEdgeX = transform.position.x + _beltCollider.offset.x + (direction * _beltCollider.size.x / 2);

        for (int i = itemsOnBelt.Count - 1; i >= 0; i--)
        {
            Rigidbody2D currentItemRb = itemsOnBelt[i];
            
            if (currentItemRb == null) continue;

            Rigidbody2D itemInFrontRb = (i == 0) ? null : itemsOnBelt[i - 1];
            
            MoveItemWithSpacing(currentItemRb, itemInFrontRb, beltEdgeX, direction);
        }
    }

    private void MoveItemWithSpacing(Rigidbody2D currentItem, Rigidbody2D itemInFront, float beltEdgeX, float direction)
    {
        if (currentItem == null) return;

        Collider2D currentItemCollider = currentItem.GetComponent<Collider2D>();
        if (currentItemCollider == null) return;

        float currentItemHalfWidth = currentItemCollider.bounds.size.x / 2f;
        float edgeStopPosition = beltEdgeX - (direction * currentItemHalfWidth);
        float finalStopPosition = edgeStopPosition;

        if (itemInFront != null)
        {
            Collider2D itemInFrontCollider = itemInFront.GetComponent<Collider2D>();
            if (itemInFrontCollider != null)
            {
                float itemInFrontHalfWidth = itemInFrontCollider.bounds.size.x / 2f;
                float spacingLimit = itemInFront.position.x - (direction * (itemInFrontHalfWidth + currentItemHalfWidth + minimumItemGap));

                finalStopPosition = (direction > 0)
                    ? Mathf.Min(edgeStopPosition, spacingLimit)
                    : Mathf.Max(edgeStopPosition, spacingLimit);
            }
        }

        Vector2 potentialNewPosition = currentItem.position + (Vector2.right * speed * Time.fixedDeltaTime);
        potentialNewPosition.x = (direction > 0)
            ? Mathf.Min(potentialNewPosition.x, finalStopPosition)
            : Mathf.Max(potentialNewPosition.x, finalStopPosition);
            
        currentItem.MovePosition(potentialNewPosition);
    }
}