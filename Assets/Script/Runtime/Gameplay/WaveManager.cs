using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObeserverPattern;
using AlchemyExpress.Quest;

namespace NPC
{
    public class WaveManager : MonoBehaviour, IWaveManager
    {
        [Header("Posisi Pergerakan")]
        [SerializeField] private Transform _enterPosition;
        [SerializeField] private Transform _servicePosition;
        [SerializeField] private Transform _leavePosition;

        public Transform enterPosition => _enterPosition;
        public Transform servicePosition => _servicePosition;
        public Transform leavePosition => _leavePosition;

        [Header("Pengaturan Wave")]
        [SerializeField] private List<QuestData> QuestsList;

        private List<QuestData> EasyQuestsPool;
        private List<QuestData> MediumQuestsPool;
        private List<QuestData> HardQuestsPool;

        private int _currentDay = 0;
        private int _npcsToSpawnThisDay;
        private int _npcsSpawnedThisDay;
        private List<QuestData> _questsForCurrentDay;

        private void Awake()
        {
            EasyQuestsPool = QuestsList.Where(q => q.levelDifficults == LevelDifficult.easy).ToList();
            MediumQuestsPool = QuestsList.Where(q => q.levelDifficults == LevelDifficult.medium).ToList();
            HardQuestsPool = QuestsList.Where(q => q.levelDifficults == LevelDifficult.hard).ToList();
        }

        private void OnEnable()
        {
            EventManager.Subscribe<NPCTurnFinishedEvent>(HandleNPCTurnFinished);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<NPCTurnFinishedEvent>(HandleNPCTurnFinished);
        }

        void Start()
        {
            StartNextDay();
        }

        /// <summary>
        /// Mempersiapkan dan memulai hari berikutnya.
        /// </summary>
        private void StartNextDay()
        {
            _currentDay++;
            _npcsSpawnedThisDay = 0;
            Debug.Log($"--- Memulai Hari ke-{_currentDay} ---");

            // Atur jumlah NPC dan jenis quest berdasarkan hari
            if (_currentDay == 1)
            {
                _npcsToSpawnThisDay = 3;
                _questsForCurrentDay = new List<QuestData>(EasyQuestsPool);
                Debug.Log($"Akan ada {_npcsToSpawnThisDay} pelanggan dengan quest tingkat EASY.");
            }
            else if (_currentDay == 2)
            {
                _npcsToSpawnThisDay = 4;
                _questsForCurrentDay = new List<QuestData>(EasyQuestsPool);
                _questsForCurrentDay.AddRange(MediumQuestsPool);
                Debug.Log($"Akan ada {_npcsToSpawnThisDay} pelanggan dengan quest tingkat EASY - MEDIUM.");
            }
            else
            {
                _npcsToSpawnThisDay = (5 + (_currentDay - 3));
                _questsForCurrentDay = new List<QuestData>(EasyQuestsPool);
                _questsForCurrentDay.AddRange(MediumQuestsPool);
                _questsForCurrentDay.AddRange(HardQuestsPool);
                Debug.Log($"Akan ada {_npcsToSpawnThisDay} pelanggan dengan quest tingkat EASY - HARD.");
            }

            // Memulai spawn NPC pertama untuk hari ini
            StartCoroutine(SpawnNextNPCWithDelay(1.5f));
        }
        
        /// <summary>
        /// Menangani event ketika giliran NPC selesai.
        /// </summary>
        private void HandleNPCTurnFinished(NPCTurnFinishedEvent e)
        {
            // Cek apakah masih ada NPC yang harus di-spawn untuk hari ini
            if (_npcsSpawnedThisDay < _npcsToSpawnThisDay)
            {
                // Jika ya, spawn NPC berikutnya
                StartCoroutine(SpawnNextNPCWithDelay(2f));
            }
            else
            {
                // Jika tidak, berarti hari ini selesai. Mulai hari berikutnya setelah jeda.
                Debug.Log($"Semua pelanggan untuk hari ke-{_currentDay} telah dilayani. Mempersiapkan hari berikutnya...");
                StartCoroutine(StartNextDayAfterDelay(5f)); // Jeda 5 detik sebelum hari baru
            }
        }

        private IEnumerator StartNextDayAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartNextDay();
        }

        /// <summary>
        /// Logika utama untuk memilih quest dan men-spawn NPC.
        /// </summary>
        private void SpawnNextNPC()
        {
            // Pastikan quest untuk hari ini masih tersedia
            if (_questsForCurrentDay != null && _questsForCurrentDay.Count > 0)
            {
                _npcsSpawnedThisDay++;

                int randomIndex = Random.Range(0, _questsForCurrentDay.Count);
                QuestData randomQuest = _questsForCurrentDay[randomIndex];
                _questsForCurrentDay.RemoveAt(randomIndex); 

                Debug.Log($"Spawning NPC #{_npcsSpawnedThisDay}/{_npcsToSpawnThisDay} untuk hari ke-{_currentDay}.");
                EventManager.Raise(new RequestNPCSpawnEvent { questData = randomQuest });
            }
            else
            {
                Debug.LogWarning("Quest untuk hari ini telah habis! Tidak ada NPC baru yang bisa di-spawn.");
                return;
            }
        }

        private IEnumerator SpawnNextNPCWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnNextNPC();
        }
    }
}