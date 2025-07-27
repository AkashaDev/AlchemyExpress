using UnityEngine;
using UnityEngine.UI;
using TMPro; // Pastikan untuk menggunakan TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("Elemen UI")]
    public GameObject chatBubbleObject; // GameObject induk dari gelembung chat
    public TextMeshProUGUI dialogueText; // Komponen teks untuk dialog
    public TextMeshProUGUI moodIndicatorText; // Komponen teks untuk emoji mood

    void Start()
    {
        // Sembunyikan saat mulai
        chatBubbleObject.SetActive(false);
        moodIndicatorText.text = "";
    }

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;
        chatBubbleObject.SetActive(true);
    }

    public void HideDialogue()
    {
        chatBubbleObject.SetActive(false);
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