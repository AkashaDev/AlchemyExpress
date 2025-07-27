using UnityEngine;
using System.Collections;

namespace NPC
{
    public class NPCController : MonoBehaviour
    {
        // Enum untuk merepresentasikan Mood dengan jelas
        public enum MoodState { Happy, Neutral, Angry, Gone }

        // --- Referensi & Data ---
        private NPCData npcData;
        private WaveManager waveManager;
        private UIManager uiManager;
        private SpriteRenderer spriteRenderer;

        // --- Status Internal ---
        private MoodState currentMood;
        private float moodTimer;
        public float timePerMoodLevel = 3f; // Waktu sebelum mood turun (detik)

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Fungsi setup yang dipanggil oleh WaveManager saat NPC diciptakan
        public void Setup(NPCData data, WaveManager manager, UIManager ui)
        {
            npcData = data;
            waveManager = manager;
            uiManager = ui;

            // Setup awal
            spriteRenderer.sprite = npcData.npcSprite;
            currentMood = MoodState.Happy;
            moodTimer = timePerMoodLevel;

            // Tampilkan dialog dan mood awal
            uiManager.ShowDialogue(npcData.dialogue);
            UpdateMoodUI();
        }

        void Update()
        {
            if (currentMood == MoodState.Gone) return;

            // Timer untuk menurunkan mood
            moodTimer -= Time.deltaTime;
            if (moodTimer <= 0)
            {
                DegradeMood();
            }
        }

        public void ReceivePotion(string playerPotionName)
        {
            if (currentMood == MoodState.Gone) return;

            uiManager.HideDialogue();

            if (playerPotionName.ToLower() == npcData.requiredPotionName.ToLower())
            {
                HandleCorrectPotion();
            }
            else
            {
                HandleWrongPotion();
            }
        }

        private void HandleCorrectPotion()
        {
            int reward = 0;
            switch (currentMood)
            {
                case MoodState.Happy:
                    reward = npcData.rewardHappy;
                    Debug.Log($"Player menerima ramuan yang benar! Reward: {reward} gold.");
                    break;
                case MoodState.Neutral:
                    reward = npcData.rewardNeutral;
                    Debug.Log($"Player menerima ramuan yang benar! Reward: {reward} gold.");
                    break;
                case MoodState.Angry:
                    reward = npcData.rewardAngry;
                    Debug.Log($"Player menerima ramuan yang benar, tapi NPC sudah kesal. Reward: {reward} gold.");
                    break;
            }
            // Di sini Anda akan menambahkan 'reward' ke inventory player
            // Contoh: PlayerStats.Instance.AddGold(reward);
            
            Leave();
        }

        private void HandleWrongPotion()
        {
            Debug.Log("Ramuan yang diberikan salah!");
            DegradeMood();
            
            // Jika setelah salah moodnya belum 'Gone', tampilkan lagi dialognya
            if (currentMood != MoodState.Gone)
            {
                uiManager.ShowDialogue(npcData.dialogue);
            }
        }

        private void DegradeMood()
        {
            if (currentMood == MoodState.Happy)
            {
                currentMood = MoodState.Neutral;
                moodTimer = timePerMoodLevel;
            }
            else if (currentMood == MoodState.Neutral)
            {
                currentMood = MoodState.Angry;
                moodTimer = timePerMoodLevel;
            }
            else if (currentMood == MoodState.Angry)
            {
                Debug.Log("NPC sudah terlalu marah dan pergi!");
                Leave();
            }
            UpdateMoodUI();
        }

        private void Leave()
        {
            currentMood = MoodState.Gone;
            UpdateMoodUI();
            uiManager.HideDialogue();
            
            waveManager.NPCHasLeft();

            Destroy(gameObject, 0.5f);
        }

        private void UpdateMoodUI()
        {
            uiManager.UpdateMoodUI(currentMood);
        }
    }
}
