using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Data", menuName = "Alchemy/NPC Data")]

namespace NPC
{
    public class NPCData : ScriptableObject
    {
        [Header("Informasi Dasar NPC")]
        public string npcName;
        public Sprite npcSprite;

        [Header("Dialog & Potion")]
        [Tooltip("Gunakan <b>kalimat</b> untuk membuat teks menjadi tebal sebagai petunjuk.")]
        [TextArea(4, 8)]
        public string dialogue;
        public string requiredPotionName;

        [Header("Reward Berdasarkan Mood")]
        public int rewardHappy;
        public int rewardNeutral;
        public int rewardAngry;
    }
}
