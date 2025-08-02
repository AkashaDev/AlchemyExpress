using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObeserverPattern;
using AlchemyExpress.Quest;

public class IngredientSpawner : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject ingredientPrefab;
    
    [Tooltip("Kolam berisi SEMUA kemungkinan bahan yang bisa muncul sebagai pengisi acak.")]
    public IngredientSO[] allIngredientsPool;

    [Header("Referensi Komponen")]
    [SerializeField] private ConveyorBelt targetConveyor;
    private Transform _spawnPoint;
    private float _spawnInterval;
    private Coroutine _spawnLoopCoroutine;

    private List<IngredientSO> _neededIngredients = new List<IngredientSO>();
    private List<IngredientSO> _fillerIngredients = new List<IngredientSO>();

    public void SetSpawnPoint(Transform newPoint) => _spawnPoint = newPoint;
    public void SetSpawnInterval(float newInterval) => _spawnInterval = newInterval;

    private void Awake()
    {
        _fillerIngredients.AddRange(allIngredientsPool);
    }

    private void OnEnable()
    {
        EventManager.Subscribe<RequestNPCSpawnEvent>(HandleNewQuest);
        EventManager.Subscribe<RequestNPCQuitEvent>(HandleNPCQuit);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<RequestNPCSpawnEvent>(HandleNewQuest);
        EventManager.Unsubscribe<RequestNPCQuitEvent>(HandleNPCQuit);
    }

    /// <summary>
    /// Menambah bahan ke daftar prioritas saat NPC datang.
    /// </summary>
    private void HandleNewQuest(RequestNPCSpawnEvent e)
    {   
        Potion quest = e.questData?.Potion;
        if (e.questData?.Potion == null) return;

        _neededIngredients.AddRange(quest.requiredIngredients);
        UpdateFillerIngredients();
        Debug.Log($"NPC datang. Bahan prioritas ditambahkan. Total prioritas: {_neededIngredients.Count}");
    }

    /// <summary>
    /// ✨ FUNGSI BARU: Menghapus bahan dari daftar prioritas saat NPC pergi.
    /// </summary>
    private void HandleNPCQuit(RequestNPCQuitEvent e)
    {
        Potion quest = e.questData?.Potion;
        if (e.questData?.Potion == null) return;

        foreach (var ingredient in quest.requiredIngredients)
        {
            if (_neededIngredients.Contains(ingredient))
            {
                _neededIngredients.Remove(ingredient);
            }
        }
        UpdateFillerIngredients(); // Perbarui lagi daftar pengisi
        Debug.Log($"NPC pergi. Bahan prioritas dihapus. Sisa prioritas: {_neededIngredients.Count}");
    }

    /// <summary>
    /// Memperbarui daftar bahan pengisi agar tidak mengandung bahan prioritas.
    /// </summary>
    private void UpdateFillerIngredients()
    {
        _fillerIngredients = allIngredientsPool.Except(_neededIngredients).ToList();
    }

    public void StartSpawning()
    {
        if (_spawnLoopCoroutine == null)
        {
            _spawnLoopCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        if (_spawnLoopCoroutine != null)
        {
            StopCoroutine(_spawnLoopCoroutine);
            _spawnLoopCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {   

            if (targetConveyor != null && targetConveyor.IsSpawnPointBlocked())
            {
                yield return _spawnInterval;
                continue;
            }
        
            IngredientSO ingredientToSpawn = null;

            if (_neededIngredients.Count > 0)
            {
                if (Random.value < 0.7f)
                {
                    int randomIndex = Random.Range(0, _neededIngredients.Count);
                    ingredientToSpawn = _neededIngredients[randomIndex];
                }
                else
                {
                    if (_fillerIngredients.Count > 0)
                    {
                        int randomIndex = Random.Range(0, _fillerIngredients.Count);
                        ingredientToSpawn = _fillerIngredients[randomIndex];
                    }
                }
            }
            else
            {
                if (allIngredientsPool.Length > 0)
                {
                    int randomIndex = Random.Range(0, allIngredientsPool.Length);
                    ingredientToSpawn = allIngredientsPool[randomIndex];
                }
            }
            
            if (ingredientToSpawn != null)
            {
                SpawnIngredient(ingredientToSpawn);
            }
            
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnIngredient(IngredientSO selectedIngredient)
    {
        if (selectedIngredient == null || _spawnPoint == null) return;
        GameObject obj = Instantiate(ingredientPrefab, _spawnPoint.position, Quaternion.identity);
        obj.GetComponent<IngredientInstance>()?.Setup(selectedIngredient);
    }
}