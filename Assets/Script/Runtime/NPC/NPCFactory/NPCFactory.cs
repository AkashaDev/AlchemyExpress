using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ObeserverPattern;

namespace NPC
{
    public class NPCFactory : MonoBehaviour
    {
        [Header("Komponen & Prefab Inti")]
        [SerializeField] private GameObject npcPrefab;
        
        [Header("Penyedia Data Scene")]
        [SerializeField] private MonoBehaviour waveManagerProvider;
        private IWaveManager waveManager;

        [Header("Pool Tampilan NPC")]
        [SerializeField] private List<AppearanceData> maleAppearances;
        [SerializeField] private List<AppearanceData> femaleAppearances;

        private List<AppearanceData> allAppearances;

        private void Awake()
        { 
            allAppearances = maleAppearances.Concat(femaleAppearances).ToList();
        }

        private void Start()
        {
            if (waveManagerProvider != null)
            {
                waveManager = waveManagerProvider.GetComponent<IWaveManager>();
            }
            if (waveManager == null)
            {
                Debug.LogError("waveManager tidak ditemukan di NPCFactory!", this.gameObject);
            }
        }
        
        private void OnEnable()
        {
            EventManager.Subscribe<RequestNPCSpawnEvent>(HandleNPCSpawnRequest);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<RequestNPCSpawnEvent>(HandleNPCSpawnRequest);
        }

        private void HandleNPCSpawnRequest(RequestNPCSpawnEvent e)
        {
            if (waveManager == null) return;
            Create(e.questData);
        }

        private NPCController Create(QuestData quest)
        {
            // --- PERBAIKAN ---
            // Ambil semua posisi yang dibutuhkan dari WaveManager
            int randomIndex = Random.Range(0, allAppearances.Count);
            AppearanceData randomAppearance = allAppearances[randomIndex];

            Transform enterPos = waveManager.enterPosition; 
            Transform servicePos = waveManager.servicePosition;
            Transform leavePos = waveManager.leavePosition;
            
            GameObject npcInstance = Instantiate(npcPrefab, enterPos.position, Quaternion.identity);
            NPCController npcController = npcInstance.GetComponent<NPCController>();
            
            // Kirim semua data yang dibutuhkan NPC, termasuk posisi
            npcController.Initialize(quest, randomAppearance, servicePos, leavePos);

            return npcController;
        }
    }
}