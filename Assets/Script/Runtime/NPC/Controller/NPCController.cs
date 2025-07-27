using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace NPC
{
        public class NPCController : MonoBehaviour
        {

        [Header("Referensi Komponen")]
        public Transform visualsTransform;
        public Transform headTransform;

        [Header("Referensi Sprite Renderer")]
        public SpriteRenderer bodyRenderer;
        public SpriteRenderer hairRenderer; 
        public SpriteRenderer faceRenderer;

        [Header("Referensi Data NPC")]
        private QuestData questData;
        private AppearanceData appearanceData;
        private WaveManager waveManager;
        private UIManager uiManager;
        [SerializeField] private float highopHeight;

        // --- Status Internal ---
        public enum MoodState { Happy, Neutral, Angry, Gone }
        private MoodState currentMood;
        private float moodTimer;
        public float timePerMoodLevel = 25f;
        private bool hasLeft = false;
        private Tween walkAnimation;
        private Tween headAnimation;
        private bool doneWalking = false;


        // Fungsi setup yang dipanggil oleh WaveManager
        public void Setup(QuestData quest, AppearanceData appearance, WaveManager manager, UIManager ui, Transform targetPosition)
        {
            // Simpan data
            this.questData = quest;
            this.appearanceData = appearance;
            this.waveManager = manager;
            this.uiManager = ui;
            hasLeft = false;

            // Terapkan semua sprite dari AppearanceData
            bodyRenderer.sprite = appearance.bodySprite;
            hairRenderer.sprite = appearance.hairSprite;
            
            // Setup mood awal dan visual wajah
            currentMood = MoodState.Happy;
            moodTimer = timePerMoodLevel;
            UpdateMoodAndVisuals();

            // Gerakan masuk
            transform.DOMove(targetPosition.position, 2.5f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                StopWalkAnimation();
                uiManager.ShowDialogue(quest.dialogue);
                doneWalking = true;
                moodTimer += 2.5f;

                StartHeadAnimation();
            });
            
            StartWalkAnimation(highopHeight, 0.4f);
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
            if (playerPotionName.ToLower() == questData.requiredPotionName.ToLower())
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
                case MoodState.Happy: reward = questData.rewardHappy; break;
                case MoodState.Neutral: reward = questData.rewardNeutral; break;
                case MoodState.Angry: reward = questData.rewardAngry; break;
            }
            Debug.Log($"Ramuan Benar! Player mendapat {reward} gold.");
            Leave();
        }

        private void HandleWrongPotion()
        {
            Debug.Log("Ramuan Salah!");
            DegradeMood();

            if (!hasLeft)
            {
                uiManager.ShowDialogue(questData.dialogue);
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
                return; 
            }

            moodTimer = timePerMoodLevel;
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
            StopHeadAnimation();
            StartWalkAnimation(highopHeight,0.4f);
        }

        private void StartHeadAnimation()
        {
            float tiltAngle = 5f;
            float tiltDuration = 1.5f;

            headAnimation = headTransform.DORotate(new Vector3(0, 0, tiltAngle), tiltDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(headTransform.gameObject); 
        }

        private void StopHeadAnimation()
        {
            if (headAnimation != null)
            {
                headAnimation.Kill();
            }
        }

        private void Flip()
        {
            float targetScaleX = transform.localScale.x * -1;
            transform.DOScaleX(targetScaleX, 0.4f).SetEase(Ease.OutSine);
        }

        private void UpdateMoodAndVisuals()
        {
            uiManager.UpdateMoodUI(currentMood);
            switch (currentMood)
            {
                case MoodState.Happy: faceRenderer.sprite = appearanceData.happyFaceSprite; break;
                case MoodState.Neutral: faceRenderer.sprite = appearanceData.neutralFaceSprite; break;
                case MoodState.Angry: faceRenderer.sprite = appearanceData.angryFaceSprite; break;
                case MoodState.Gone: ; break;
            }
        }
    
    private void StartWalkAnimation(float hopHeight, float oneWayDuration)
    {
        // Hentikan animasi lama jika ada
        if (walkAnimation != null) walkAnimation.Kill();
        
            walkAnimation = visualsTransform.DOLocalMoveY(transform.localPosition.y + hopHeight, oneWayDuration)
            .SetEase(Ease.OutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(visualsTransform.gameObject); 
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
