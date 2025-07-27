using UnityEngine;
using System.Collections;
using TMPro;

namespace NPC
{
    public class UIManager : MonoBehaviour
    {   
        [Header("Elemen UI")]
        public GameObject chatBubbleObject;
        public TextMeshProUGUI dialogueText;
        public TextMeshProUGUI moodIndicatorText;

        [Header("Pengaturan Typewriter")]
        [Tooltip("Kecepatan munculnya huruf per detik")]
        public float charactersPerSecond = 50f;
        private Coroutine typingCoroutine;

        void Start()
        {
            chatBubbleObject.SetActive(false);
            moodIndicatorText.text = "";
        }

        public void ShowDialogue(string text)
        {
            chatBubbleObject.SetActive(true);

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeDialogue(text));
        }

        public void HideDialogue()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            chatBubbleObject.SetActive(false);
        }

        private IEnumerator TypeDialogue(string text)
        {
            dialogueText.text = "";
            
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    int closingTagIndex = text.IndexOf('>', i);
                    if (closingTagIndex != -1)
                    {
                        string tag = text.Substring(i, closingTagIndex - i + 1);
                        dialogueText.text += tag;
                        
                        i = closingTagIndex;
                        
                        continue;
                    }
                }
                
                // Jika bukan tag, ketik huruf seperti biasa
                dialogueText.text += text[i];
                yield return new WaitForSeconds(1f / charactersPerSecond);
            }
        }

        public void UpdateMoodUI(NPCController.MoodState mood)
        {
            switch (mood)
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
    }
}
