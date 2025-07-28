using UnityEngine;

public class ConveyorVisuals : MonoBehaviour
{
    [Tooltip("Kecepatan dan arah scrolling tekstur. Nilai positif untuk ke kanan, negatif untuk ke kiri.")]
    [SerializeField] private float scrollSpeed = 0.5f;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        float newOffsetX = _renderer.material.mainTextureOffset.x + scrollSpeed * Time.deltaTime;

        Vector2 newOffset = new Vector2(newOffsetX, 0);
        _renderer.material.mainTextureOffset = newOffset;
    }
}