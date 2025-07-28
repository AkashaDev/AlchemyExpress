using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using ObeserverPattern;

namespace NPC
{
    public class WaveManager : MonoBehaviour, IWaveManager
    {
        [Header("Posisi Pergerakan")]
        [SerializeField] private Transform _enterPosition;
        [SerializeField] private Transform _servicePosition;
        [SerializeField] private Transform _leavePosition;

        // Implementasi dari interface IWaveManager
        public Transform enterPosition => _enterPosition;
        public Transform servicePosition => _servicePosition;
        public Transform leavePosition => _leavePosition;
        
        [Header("Pengaturan Wave")]
        [SerializeField] private List<QuestData> wave1Quests;

        private List<QuestData> availableQuestsInWave;

        private void OnEnable()
        {
            EventManager.Subscribe<NPCTurnFinishedEvent>(HandleNPCTurnFinished);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<NPCTurnFinishedEvent>(HandleNPCTurnFinished);
        }

        private void HandleNPCTurnFinished(NPCTurnFinishedEvent e)
        {
            StartCoroutine(SpawnNextNPCWithDelay(2f));
        }

        void Start()
        {
            StartWave(1);
        }

        public void StartWave(int waveNumber)
        {
            if (waveNumber == 1 )
            {
                availableQuestsInWave = new List<QuestData>(wave1Quests);
            }
            StartCoroutine(SpawnNextNPCWithDelay(1f));
        }

        private void SpawnNextNPC()
        {
            if (availableQuestsInWave != null && availableQuestsInWave.Count > 0)
            {
                int randomIndex = Random.Range(0, availableQuestsInWave.Count);
                QuestData randomQuest = availableQuestsInWave[randomIndex];
                availableQuestsInWave.RemoveAt(randomIndex);
                EventManager.Raise(new RequestNPCSpawnEvent { questData = randomQuest });
            }
            else
            {
                Debug.Log("Wave selesai! Semua quest telah diberikan.");
            }
        }
        
        private IEnumerator SpawnNextNPCWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnNextNPC();
        }
    }
}