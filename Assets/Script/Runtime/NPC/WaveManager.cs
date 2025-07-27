using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace NPC
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Referensi Utama")]
        public UIManager uiManager;
        public GameObject npcPrefab;

        [Header("Posisi Pergerakan")]
        public Transform enterPosition; 
        public Transform servicePosition; 
        public Transform leavePosition;

        [Header("Pengaturan Wave")]
        public List<NPCData> wave1Npcs;
        // public List<NPCData> wave2Npcs;

        private Queue<NPCData> currentWaveQueue;
        public GameObject currentNpcObject { get; private set; }

        void Start()
        {
            StartWave(1);
        }

        public void StartWave(int waveNumber)
        {
            Debug.Log($"Memulai Wave {waveNumber}!");
            currentWaveQueue = new Queue<NPCData>();
            List<NPCData> selectedWave = (waveNumber == 1) ? wave1Npcs : null;
            
            if (selectedWave != null)
            {
                foreach (var npc in selectedWave)
                {
                    currentWaveQueue.Enqueue(npc);
                }
            }
            
            StartCoroutine(SpawnNextNPCWithDelay(1f));
        }

        private void SpawnNextNPC()
        {
            if (currentWaveQueue.Count > 0)
            {
                NPCData nextNpcData = currentWaveQueue.Dequeue();
                currentNpcObject = Instantiate(npcPrefab, enterPosition.position, Quaternion.identity);
                
                NPCController npcController = currentNpcObject.GetComponent<NPCController>();
                npcController.Setup(nextNpcData, this, uiManager, servicePosition);
            }
            else
            {
                Debug.Log("Wave selesai!");
            }
        }

        // Fungsi ini dipanggil SETELAH NPC selesai pergi
        public void OnNPCHasLeft()
        {
            currentNpcObject = null;
            StartCoroutine(SpawnNextNPCWithDelay(2f));
        }
        
        private IEnumerator SpawnNextNPCWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnNextNPC();
        }
    }
}
