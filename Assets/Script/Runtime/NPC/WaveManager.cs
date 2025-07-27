using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace NPC
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Referensi Prefab & UI")]
        public GameObject npcPrefab; // Prefab NPC yang sudah ada NPCController
        public Transform npcSpawnPoint; // Posisi di mana NPC akan muncul
        public UIManager uiManager; // Referensi ke UI Manager

        [Header("Pengaturan Wave")]
        public List<NPCData> wave1Npcs; // Daftar data NPC untuk wave 1
        public List<NPCData> wave2Npcs; // Daftar data NPC untuk wave 2
        // Tambahkan list lain untuk wave selanjutnya

        private Queue<NPCData> currentWaveQueue;
        private GameObject currentNpcObject;

        void Start()
        {
            // Contoh: Memulai wave pertama saat game dimulai
            StartWave(1);
        }

        public void StartWave(int waveNumber)
        {
            Debug.Log($"Memulai Wave {waveNumber}!");
            currentWaveQueue = new Queue<NPCData>();

            List<NPCData> selectedWave = null;
            if (waveNumber == 1) selectedWave = wave1Npcs;
            if (waveNumber == 2) selectedWave = wave2Npcs;
            // Tambahkan kondisi lain untuk wave selanjutnya

            if (selectedWave != null)
            {
                foreach (var npc in selectedWave)
                {
                    currentWaveQueue.Enqueue(npc);
                }
            }

            // Panggil NPC pertama
            SpawnNextNPC();
        }

        private void SpawnNextNPC()
        {
            if (currentWaveQueue.Count > 0)
            {
                NPCData nextNpcData = currentWaveQueue.Dequeue();
                currentNpcObject = Instantiate(npcPrefab, npcSpawnPoint.position, Quaternion.identity);

                // Mengirimkan semua data dan referensi yang dibutuhkan ke NPC baru
                NPCController npcController = currentNpcObject.GetComponent<NPCController>();
                npcController.Setup(nextNpcData, this, uiManager);
            }
            else
            {
                Debug.Log("Wave selesai!");
                // Di sini Anda bisa menambahkan logika untuk akhir wave (misal: menampilkan skor)
            }
        }

        // Fungsi ini dipanggil oleh NPCController saat ia pergi
        public void NPCHasLeft()
        {
            currentNpcObject = null;
            // Tunggu sebentar sebelum memanggil NPC selanjutnya
            StartCoroutine(SpawnNextNPCWithDelay(2f));
        }

        private IEnumerator SpawnNextNPCWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnNextNPC();
        }
    }
}
