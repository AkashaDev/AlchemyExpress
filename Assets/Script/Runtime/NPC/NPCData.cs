using UnityEngine;

namespace NPC
{   
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "Alchemy/NPC Data")]
    public class NPCData : ScriptableObject
    {
        [Header("Informasi Dasar NPC")]
        public string npcName;

        [Header("Komponen Sprite")]
        public Sprite bodySprite;
        public Sprite headSprite;
        
        [Header("Sprite Wajah Sesuai Emosi")]
        public Sprite happyFaceSprite; 
        public Sprite neutralFaceSprite;
        public Sprite angryFaceSprite;

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
