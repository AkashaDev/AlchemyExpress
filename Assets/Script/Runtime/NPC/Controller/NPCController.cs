using UnityEngine;
using System.Collections;
using DG.Tweening;
using ObeserverPattern;

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
        // private WaveManager waveManager;

        private Transform servicePosition;
        private Transform leavePosition;

        private UIManager uiManager;
        [SerializeField] private float highopHeight;
        private Transform leaveTarget;

        // --- Status Internal ---
        public enum MoodState { Happy, Neutral, Angry, Gone }
        private MoodState currentMood;
        private float moodTimer;
        public float timePerMoodLevel = 25f;
        private bool hasLeft = false;
        private Tween walkAnimation;
        private Tween headAnimation;
        private bool isWalking = false;

        public void Initialize(QuestData quest, AppearanceData appearance, Transform servicePos, Transform leavePos)
        {
            this.questData = quest;
            this.appearanceData = appearance;
            this.servicePosition = servicePos;
            this.leavePosition = leavePos;
        }
        
        private void Start()
        {
            hasLeft = false;

            // Terapkan data visual dan mood
            bodyRenderer.sprite = appearanceData.bodySprite;
            hairRenderer.sprite = appearanceData.hairSprite;
            currentMood = MoodState.Happy;
            moodTimer = timePerMoodLevel;
            UpdateMoodAndVisuals();

            // Mulai bergerak ke posisi service
            Transform targetPosition = FindObjectOfType<WaveManager>().servicePosition;
            transform.DOMove(targetPosition.position, 2.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                StopWalkAnimation();
                EventManager.Raise(new ShowDialogueEvent { dialogueText = questData.dialogue });
                StartHeadAnimation();
                moodTimer += 2.5f;
                isWalking = true;
            });

            StartWalkAnimation(highopHeight, 0.4f);
        }

        void Update()
        {
            if (hasLeft && !isWalking) return;

            moodTimer -= Time.deltaTime;
            if (moodTimer <= 0)
            {
                DegradeMood();
            }
        }

         public void ReceivePotion(string playerPotionName)
        {
            if (hasLeft) return;
            EventManager.Raise(new HideDialogueEvent());
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
                EventManager.Raise(new ShowDialogueEvent{ dialogueText = questData.dialogue });
            }
        }

        private void DegradeMood()
        {
            if (currentMood >= MoodState.Angry)
            {
                Debug.Log("NPC sudah terlalu marah dan pergi!");
                Leave();
                return;
            }
            currentMood++;

            moodTimer = timePerMoodLevel;
            UpdateMoodAndVisuals();
        }

        private void Leave()
        {
            if (hasLeft) return;
            hasLeft = true;
            isWalking = false;
            currentMood = MoodState.Gone;

            UpdateMoodAndVisuals();
            EventManager.Raise(new HideDialogueEvent());
            leaveTarget = FindObjectOfType<WaveManager>().leavePosition;
            
            transform.DOMove(leaveTarget.position, 2.5f).SetEase(Ease.Linear).OnComplete(() =>
            {

                EventManager.Raise(new NPCTurnFinishedEvent());
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
            EventManager.Raise(new UpdateNPCMoodEvent { newMood = currentMood });
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

        float localPositionY = FindObjectOfType<WaveManager>().servicePosition.transform.localPosition.y;
        
        transform.DOLocalMoveY(localPositionY, 0.1f);
    }
    }
}
