using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlchemyExpress.Quest;
using ObeserverPattern;
using UnityEngine;

namespace NPC
{
    public class WaveManager : MonoBehaviour, IWaveManager
    {
        [Header("Posisi Pergerakan")]
        [SerializeField]
        private Transform _enterPosition;

        [SerializeField]
        private Transform _servicePosition;

        [SerializeField]
        private Transform _leavePosition;

        public Transform enterPosition => _enterPosition;
        public Transform servicePosition => _servicePosition;
        public Transform leavePosition => _leavePosition;

        [Header("Pengaturan Wave")]
        [SerializeField]
        private List<QuestData> QuestsList;

        private List<QuestData> EasyQuestsPool;
        private List<QuestData> MediumQuestsPool;
        private List<QuestData> HardQuestsPool;
        private List<QuestData> _activeQuests = new List<QuestData>();

        private int _currentDay = 0;
        private int _npcsToSpawnThisDay;
        private int _npcsSpawnedThisDay;
        private List<QuestData> _questsForCurrentDay;

        private int _surplusIncomeFromPreviousDay = 0;
        private int _playerIncomeThisDay;
        private int _targetIncomeThisDay;
        private const float OBJECTIVE_PERCENTAGE = 0.6f;

        private void Awake()
        {
            EasyQuestsPool = QuestsList
                .Where(q => q.levelDifficults == LevelDifficult.easy)
                .ToList();
            MediumQuestsPool = QuestsList
                .Where(q => q.levelDifficults == LevelDifficult.medium)
                .ToList();
            HardQuestsPool = QuestsList
                .Where(q => q.levelDifficults == LevelDifficult.hard)
                .ToList();
        }

        private void OnEnable()
        {
            EventManager.Subscribe<NPCTurnFinishedEvent>(HandleNPCTurnFinished);
            EventManager.Subscribe<RequestNPCQuitEvent>(HandleNPCQuit);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<NPCTurnFinishedEvent>(HandleNPCTurnFinished);
            EventManager.Unsubscribe<RequestNPCQuitEvent>(HandleNPCQuit);
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
            _playerIncomeThisDay = 0;
            _targetIncomeThisDay = 0;
            Debug.Log($"--- Memulai Hari ke-{_currentDay} ---");
            Debug.Log($"Bonus pendapatan dari hari sebelumnya: {_surplusIncomeFromPreviousDay}");

            if (_currentDay == 1)
            {
                _npcsToSpawnThisDay = 3;
            }
            else if (_currentDay == 2)
            {
                _npcsToSpawnThisDay = 4;
            }
            else
            {
                _npcsToSpawnThisDay = (5 + (_currentDay - 3));
            }
            
            var potentialQuestsPool = new List<QuestData>();
            if (_currentDay >= 1) potentialQuestsPool.AddRange(EasyQuestsPool);
            if (_currentDay >= 2) potentialQuestsPool.AddRange(MediumQuestsPool);
            if (_currentDay >= 3) potentialQuestsPool.AddRange(HardQuestsPool);

            if(potentialQuestsPool.Count == 0)
            {
                Debug.LogError("Tidak ada quest yang tersedia di pool!");
                return;
            }

            _questsForCurrentDay = potentialQuestsPool
                .OrderBy(q => System.Guid.NewGuid())
                .Take(_npcsToSpawnThisDay)
                .ToList();
                
            Debug.Log($"Akan ada {_npcsToSpawnThisDay} pelanggan hari ini.");

            _targetIncomeThisDay = _questsForCurrentDay.Sum(quest => quest.rewardHappy);
            List<QuestData> availableQuestsInWave = new List<QuestData>(_questsForCurrentDay);

            float requiredIncome = _targetIncomeThisDay * OBJECTIVE_PERCENTAGE;
            float effectiveRequiredIncome = Mathf.Max(0, requiredIncome - _surplusIncomeFromPreviousDay);

            Debug.Log($"Target pendapatan mentah: {requiredIncome} (dari total {_targetIncomeThisDay}). Pendapatan yang perlu dicari hari ini: {effectiveRequiredIncome}");

            StartCoroutine(SpawnNextNPCWithDelay(1.5f));
        }

        /// <summary>
        /// Menangani event ketika giliran NPC selesai.
        /// </summary>
        private void HandleNPCTurnFinished(NPCTurnFinishedEvent e)
        {   
            _playerIncomeThisDay += e.rewardEarned;
            if (_npcsSpawnedThisDay >= _npcsToSpawnThisDay)
            {
                CheckEndOfDayObjective();
            }
            else
            {
                StartCoroutine(SpawnNextNPCWithDelay(2f));
            }
        }

        private void CheckEndOfDayObjective()
        {
            int totalEarned = _playerIncomeThisDay + _surplusIncomeFromPreviousDay;
            float requiredIncome = _targetIncomeThisDay * OBJECTIVE_PERCENTAGE;

            Debug.Log(
                $"Hari ke-{_currentDay} Selesai. Total Pendapatan (termasuk bonus): {totalEarned} / {requiredIncome}."
            );

            if (totalEarned >= requiredIncome)
            {
                // SUKSES
                // Hitung kelebihan pendapatan untuk disimpan ke hari berikutnya
                _surplusIncomeFromPreviousDay = totalEarned - (int)requiredIncome;
                Debug.Log(
                    $"Objektif TERCAPAI! Bonus sebesar {_surplusIncomeFromPreviousDay} dibawa ke hari berikutnya."
                );
                StartCoroutine(StartNextDayAfterDelay(5f));
            }
            else
            {
                // GAGAL
                Debug.LogError("Objektif GAGAL! PERMAINAN BERAKHIR.");
                // Reset bonus jika ada sistem 'coba lagi'
                _surplusIncomeFromPreviousDay = 0;
                // Tambahkan logika Game Over di sini
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
            if (_questsForCurrentDay != null && _questsForCurrentDay.Count > 0)
            {
                _npcsSpawnedThisDay++;
                
                int randomIndex = Random.Range(0, _questsForCurrentDay.Count);
                QuestData randomQuest = _questsForCurrentDay[randomIndex];

                EventManager.Raise(new QuestAboutToStartEvent { questData = randomQuest });
                
                Debug.Log($"Spawning NPC #{_npcsSpawnedThisDay}/{_npcsToSpawnThisDay} untuk hari ke-{_currentDay}.");
                EventManager.Raise(new RequestNPCSpawnEvent { questData = randomQuest });
            }
            else
            {
                Debug.LogWarning("Quest untuk hari ini telah habis!");
                return;
            }
        }

        private void HandleNPCQuit(RequestNPCQuitEvent e)
        {
            if (e.questData != null && _activeQuests.Contains(e.questData))
            {
                _activeQuests.Remove(e.questData);
                EventManager.Raise(new RequestNPCSpawnEvent { questData = e.questData });
                Debug.Log(
                    $"Quest '{e.questData.name}' selesai. Sisa quest aktif: {_activeQuests.Count}"
                );
            }
        }

        private IEnumerator SpawnNextNPCWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnNextNPC();
        }
    }
}
