using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ObeserverPattern;
using AlchemyExpress.Quest;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;
    private Transform _spawnPoint;
    private float _spawnInterval;
    private Coroutine _spawnLoopCoroutine;

    private Queue<IngredientSO> _spawnQueue = new Queue<IngredientSO>();

    public void SetSpawnPoint(Transform newPoint) => _spawnPoint = newPoint;
    public void SetSpawnInterval(float newInterval) => _spawnInterval = newInterval;

    private void OnEnable()
    {
        EventManager.Subscribe<RequestNPCSpawnEvent>(HandleNewQuest);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<RequestNPCSpawnEvent>(HandleNewQuest);
    }

    private void HandleNewQuest(RequestNPCSpawnEvent e)
    {
        if (e.questData?.requiredIngredients == null) return;
        foreach (var ingredient in e.questData.requiredIngredients)
        {
            _spawnQueue.Enqueue(ingredient);
        }
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
            if (_spawnQueue.Count > 0)
            {
                IngredientSO ingredientToSpawn = _spawnQueue.Dequeue();
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