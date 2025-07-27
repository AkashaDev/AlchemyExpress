using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace NPC
{
    public class NPCController : MonoBehaviour
    {   
        [Header("Referensi Komponen")]
        public Transform visualsTransform;
        public enum MoodState { Happy, Neutral, Angry, Gone }

        [Header("Referensi Sprite Renderer")]
        public SpriteRenderer bodyRenderer;
        public SpriteRenderer headRenderer;
        public SpriteRenderer faceRenderer;

        // --- Referensi & Data ---
        private NPCData npcData;
        private WaveManager waveManager;
        private UIManager uiManager;
        [SerializeField] private float highopHeight;

        // --- Status Internal ---
        private MoodState currentMood;
        private float moodTimer;
        public float timePerMoodLevel = 25f;
        private bool hasLeft = false;
        private Tween walkAnimation;
        private bool doneWalking = false;


        // Fungsi setup yang dipanggil oleh WaveManager
        public void Setup(NPCData data, WaveManager manager, UIManager ui, Transform targetPosition)
        {
            npcData = data;
            waveManager = manager;
            uiManager = ui;
            hasLeft = false;

            bodyRenderer.sprite = npcData.bodySprite;
            headRenderer.sprite = npcData.headSprite;

            currentMood = MoodState.Happy;
            moodTimer = timePerMoodLevel;
            UpdateMoodAndVisuals();

            StartWalkAnimation(highopHeight, 0.4f);

            transform.DOMove(targetPosition.position, 2.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                doneWalking = true;
                moodTimer += 2.5f;
                StopWalkAnimation();
                uiManager.ShowDialogue(npcData.dialogue);

            });

        }

        void Update()
        {
            if (hasLeft && !doneWalking) return;

            moodTimer -= Time.deltaTime;
            if (moodTimer <= 0)
            {
                DegradeMood();
            }
        }

        public void ReceivePotion(string playerPotionName)
        {
            if (hasLeft) return;

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
                case MoodState.Happy: reward = npcData.rewardHappy; break;
                case MoodState.Neutral: reward = npcData.rewardNeutral; break;
                case MoodState.Angry: reward = npcData.rewardAngry; break;
            }
            Debug.Log($"Ramuan Benar! Player mendapat {reward} gold.");
            // PlayerStats.Instance.AddGold(reward);

            Leave();
        }

        private void HandleWrongPotion()
        {
            Debug.Log("Ramuan Salah!");
            DegradeMood();

            if (!hasLeft)
            {
                uiManager.ShowDialogue(npcData.dialogue);
            }
        }

        private void DegradeMood()
        {
            if (currentMood == MoodState.Happy) currentMood = MoodState.Neutral;
            else if (currentMood == MoodState.Neutral) currentMood = MoodState.Angry;
            else if (currentMood == MoodState.Angry)
            {
                Debug.Log("NPC sudah terlalu marah dan pergi!");
                Leave();
                return; // Hentikan fungsi agar tidak reset timer
            }

            moodTimer = timePerMoodLevel; // Reset timer
            UpdateMoodAndVisuals();
        }

        private void Leave()
        {
            if (hasLeft) return;
            hasLeft = true;
            currentMood = MoodState.Gone;

            UpdateMoodAndVisuals();
            uiManager.HideDialogue();
            
            transform.DOMove(waveManager.leavePosition.position, 2.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                waveManager.OnNPCHasLeft();
                Destroy(gameObject);
            });

            Flip();
            StartWalkAnimation(highopHeight,0.4f);
        }

        private void Flip()
        {
            float targetScaleX = transform.localScale.x * -1;
            transform.DOScaleX(targetScaleX, 0.4f).SetEase(Ease.OutSine);
        }

        // Fungsi ini sekarang mengurus UI dan sprite wajah
        private void UpdateMoodAndVisuals()
        {
            uiManager.UpdateMoodUI(currentMood); // Update emoji di UI

            // Ganti sprite wajah berdasarkan mood
            switch (currentMood)
            {
                case MoodState.Happy:
                    faceRenderer.sprite = npcData.happyFaceSprite;
                    break;
                case MoodState.Neutral:
                    faceRenderer.sprite = npcData.neutralFaceSprite;
                    break;
                case MoodState.Angry:
                    faceRenderer.sprite = npcData.angryFaceSprite;
                    break;
                case MoodState.Gone:
                    // faceRenderer.sprite = null; // Sembunyikan wajah saat pergi
                    break;
            }
        }
    
    private void StartWalkAnimation(float hopHeight, float oneWayDuration)
    {
        // Hentikan animasi lama jika ada
        if (walkAnimation != null) walkAnimation.Kill();
        
            walkAnimation = visualsTransform.DOLocalMoveY(transform.localPosition.y + hopHeight, oneWayDuration)
            .SetEase(Ease.OutSine) // Ease untuk gerakan naik & turun
            .SetLoops(-1, LoopType.Yoyo); // Loop tak terbatas, bolak-balik (naik-turun)
    }

    private void StopWalkAnimation()
    {
        if (walkAnimation != null)
        {
            walkAnimation.Kill();
        }
        transform.DOLocalMoveY(waveManager.servicePosition.transform.localPosition.y, 0.1f);
    }
    }
}
