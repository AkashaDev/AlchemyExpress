using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Dibutuhkan untuk memfilter
using ObeserverPattern;
using AlchemyExpress.Quest;

public class IngredientSpawner : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject ingredientPrefab;
    [Tooltip("Kolam berisi SEMUA kemungkinan bahan yang bisa muncul sebagai pengisi acak.")]
    public IngredientSO[] allIngredientsPool;

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
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<RequestNPCSpawnEvent>(HandleNewQuest);
    }

    /// <summary>
    /// Menangani quest baru dengan menambahkan bahannya ke daftar 'needed'.
    /// </summary>
    private void HandleNewQuest(RequestNPCSpawnEvent e)
    {
        if (e.questData?.requiredIngredients == null) return;

        Debug.Log($"Quest baru diterima. Menambahkan {e.questData.requiredIngredients.Count} bahan yang dibutuhkan.");
        _neededIngredients.AddRange(e.questData.requiredIngredients);

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

    /// <summary>
    /// Loop utama dengan sistem probabilitas 50/50.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            IngredientSO ingredientToSpawn = null;

            if (_neededIngredients.Count > 0)
            {
                if (Random.value < 0.5f)
                {
                    int randomIndex = Random.Range(0, _neededIngredients.Count);
                    ingredientToSpawn = _neededIngredients[randomIndex];
                    _neededIngredients.RemoveAt(randomIndex);
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