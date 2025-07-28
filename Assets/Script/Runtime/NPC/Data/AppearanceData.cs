using UnityEngine;

namespace NPC
{ 
    [CreateAssetMenu(fileName = "Appearance_", menuName = "Alchemy/Appearance Data")]
    public class AppearanceData : ScriptableObject
    {
        public enum Gender { LakiLaki, Perempuan }

        [Header("Karakteristik Visual")]
        public Gender gender;
        public Sprite bodySprite; // Sprite tubuh dasar (bisa beda untuk L/P)
        public Sprite hairSprite;
        
        [Header("Wajah Sesuai Emosi")]
        public Sprite happyFaceSprite;
        public Sprite neutralFaceSprite;
        public Sprite angryFaceSprite;
    }
}

