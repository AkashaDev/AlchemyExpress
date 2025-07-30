using UnityEngine;
using System.Collections;
using TMPro;
using ObeserverPattern;

namespace NPC
{
    public class UIManager : MonoBehaviour
    {
        [Header("Elemen UI")]
        [SerializeField] private GameObject chatBubbleObject;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI dialogueTextAfter;
        [SerializeField] private TextMeshProUGUI moodIndicatorText;

        [Header("Pengaturan Typewriter")]
        [SerializeField] private float charactersPerSecond = 50f;

        private Coroutine typingCoroutine;

        private void OnEnable()
        {
            // Berlangganan ke semua event yang relevan
            EventManager.Subscribe<ShowDialogueEvent>(HandleShowDialogue);
            EventManager.Subscribe<HideDialogueEvent>(HandleHideDialogue);
            EventManager.Subscribe<UpdateNPCMoodEvent>(HandleUpdateMood);
        }

        private void OnDisable()
        {
            // Berhenti langganan untuk mencegah error
            EventManager.Unsubscribe<ShowDialogueEvent>(HandleShowDialogue);
            EventManager.Unsubscribe<HideDialogueEvent>(HandleHideDialogue);
            EventManager.Unsubscribe<UpdateNPCMoodEvent>(HandleUpdateMood);
        }

        private void HandleShowDialogue(ShowDialogueEvent e)
        {
            chatBubbleObject.SetActive(true);
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeDialogue(e.dialogueText));
        }

        private void HandleHideDialogue(HideDialogueEvent e)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            chatBubbleObject.SetActive(false);
        }

        private void HandleUpdateMood(UpdateNPCMoodEvent e)
        {
            switch (e.newMood)
            {
                case NPCController.MoodState.Happy:
                    moodIndicatorText.text = "Senang";
                    moodIndicatorText.color = Color.green;
                    break;
                case NPCController.MoodState.Neutral:
                    moodIndicatorText.text = "Netral";
                    moodIndicatorText.color = Color.yellow;
                    break;
                case NPCController.MoodState.Angry:
                    moodIndicatorText.text = "Marah";
                    moodIndicatorText.color = Color.red;
                    break;
                case NPCController.MoodState.Gone:
                    moodIndicatorText.text = "Pergi";
                    moodIndicatorText.color = Color.grey;
                    break;
            }
        }
        
        private IEnumerator TypeDialogue(string text)
        {
            dialogueText.text = "";
            dialogueTextAfter.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    int closingTagIndex = text.IndexOf('>', i);
                    if (closingTagIndex != -1)
                    {
                        string tag = text.Substring(i, closingTagIndex - i + 1);
                        dialogueText.text += tag;
                        dialogueTextAfter.text += tag;
                        i = closingTagIndex;
                        continue;
                    }
                }
                dialogueText.text += text[i];
                dialogueTextAfter.text += text[i];
                yield return new WaitForSeconds(1f / charactersPerSecond);
            }
        }
    }
}