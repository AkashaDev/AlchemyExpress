using UnityEngine;
using System.Collections;
using TMPro;
using ObeserverPattern;
using UnityEngine.Localization; // 1. Tambahkan namespace Localization

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
        
        // 2. Tambahkan variabel untuk menyimpan referensi dialog yang sedang aktif
        private LocalizedString currentDialogueText; 

        private void OnEnable()
        {
            EventManager.Subscribe<ShowDialogueEvent>(HandleShowDialogue);
            EventManager.Subscribe<HideDialogueEvent>(HandleHideDialogue);
            EventManager.Subscribe<UpdateNPCMoodEvent>(HandleUpdateMood);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<ShowDialogueEvent>(HandleShowDialogue);
            EventManager.Unsubscribe<HideDialogueEvent>(HandleHideDialogue);
            EventManager.Unsubscribe<UpdateNPCMoodEvent>(HandleUpdateMood);
            
            // 3. Pastikan untuk unsubscribe dari event localization saat objek mati
            if (currentDialogueText != null)
            {
                currentDialogueText.StringChanged -= OnDialogueStringChanged;
            }
        }

        private void HandleShowDialogue(ShowDialogueEvent e)
        {
            chatBubbleObject.SetActive(true);
            
            // 4. Hapus langganan dari dialog lama (jika ada) untuk mencegah memory leak
            if (currentDialogueText != null)
            {
                currentDialogueText.StringChanged -= OnDialogueStringChanged;
            }

            // 5. Simpan referensi dialog baru
            currentDialogueText = e.dialogueText;

            // 6. Langganan ke event StringChanged. 
            // Fungsi OnDialogueStringChanged akan otomatis dipanggil 1x saat ini juga, 
            // dan akan dipanggil lagi jika player mengganti bahasa di pengaturan.
            if (currentDialogueText != null)
            {
                currentDialogueText.StringChanged += OnDialogueStringChanged;
            }
        }
        
        // 7. Fungsi baru ini yang akan menerima teks (string) yang sudah diterjemahkan
        private void OnDialogueStringChanged(string translatedText)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            // Mulai efek typewriter menggunakan string yang sudah diterjemahkan
            typingCoroutine = StartCoroutine(TypeDialogue(translatedText));
        }

        private void HandleHideDialogue(HideDialogueEvent e)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            chatBubbleObject.SetActive(false);
            
            // 8. Bersihkan referensi saat dialog ditutup
            if (currentDialogueText != null)
            {
                currentDialogueText.StringChanged -= OnDialogueStringChanged;
                currentDialogueText = null;
            }
        }

        private void HandleUpdateMood(UpdateNPCMoodEvent e)
        {
            // Catatan: Teks mood ini ("Senang", "Netral", "Marah") masih hardcoded. 
            // Nantinya bisa kamu ubah menjadi LocalizedString juga jika ingin diterjemahkan.
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