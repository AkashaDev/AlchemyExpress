using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

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
        public List<QuestData> wave1Quests;
        
        [Header("Pool Tampilan NPC")]
        public List<AppearanceData> maleAppearances;
        public List<AppearanceData> femaleAppearances;

        private Queue<QuestData> currentQuestQueue;
        public GameObject currentNpcObject { get; private set; }

        private List<AppearanceData> allAppearances;

        void Awake()
        {
            allAppearances = maleAppearances.Concat(femaleAppearances).ToList();
        }

        void Start()
        {
            if (allAppearances == null || allAppearances.Count == 0)
            {
                Debug.LogError("Tidak ada AppearanceData yang diatur di WaveManager!");
                return;
            }
            StartWave(1);
        }

        public void StartWave(int waveNumber)
        {
            currentQuestQueue = new Queue<QuestData>();
            
            List<QuestData> selectedWave = (waveNumber == 1) ? wave1Quests : null;
            
            if (selectedWave != null)
            {
                foreach (var quest in selectedWave)
                {
                    currentQuestQueue.Enqueue(quest);
                }
            }
            
            StartCoroutine(SpawnNextNPCWithDelay(1f));
        }

        private void SpawnNextNPC()
        {
            if (currentQuestQueue.Count > 0)
            {
                QuestData nextQuest = currentQuestQueue.Dequeue();
                int randomIndex = Random.Range(0, allAppearances.Count);
                AppearanceData randomAppearance = allAppearances[randomIndex];
            
                currentNpcObject = Instantiate(npcPrefab, enterPosition.position, Quaternion.identity);
                
                NPCController npcController = currentNpcObject.GetComponent<NPCController>();
                npcController.Setup(nextQuest, randomAppearance, this, uiManager, servicePosition);
            }
            else
            {
                Debug.Log("Wave selesai!");
            }
        }

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
