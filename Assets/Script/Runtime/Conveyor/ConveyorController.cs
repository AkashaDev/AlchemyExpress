using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [Header("Pengaturan Utama")]
    [Tooltip("Kecepatan gerak conveyor (visual dan fisik).")]
    [SerializeField] private float conveyorSpeed = 2.0f;

    [Tooltip("Berapa banyak bahan yang muncul per detik, relatif terhadap kecepatan.")]
    [SerializeField] private float itemDensity = 0.5f;

    [Header("Referensi Komponen")]
    [SerializeField] private ConveyorBelt _conveyorBelt;
    [SerializeField] private IngredientSpawner _spawner;

    void Start()
    {
        if (_conveyorBelt == null || _spawner == null)
        {
            Debug.LogError("ConveyorBelt atau IngredientSpawner belum di-assign di Inspector!", this);
            return;
        }

        _conveyorBelt.Initialize(conveyorSpeed);
        _spawner.SetSpawnPoint(_conveyorBelt.spawnPoint);

        // Logika ini tetap sama dan benar
        float spawnInterval = 1f / (conveyorSpeed * itemDensity);
        _spawner.SetSpawnInterval(spawnInterval);
        _spawner.StartSpawning();
    }
}