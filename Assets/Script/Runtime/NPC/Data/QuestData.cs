using System.Collections.Generic;
using AlchemyExpress.Quest;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest_", menuName = "Alchemy/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Detail Misi")]
    [Tooltip("Gunakan <b>kalimat</b> untuk membuat teks menjadi tebal sebagai petunjuk.")]
    [TextArea(5, 10)]
    public string dialogue;
    public Potion Potion;
    public LevelDifficult levelDifficults;

    [Header("Reward Berdasarkan Mood")]
    public int rewardHappy;
    public int rewardNeutral;
    public int rewardAngry;

    [Header("Pengaturan Gameplay")]
    public CauldronTemplateSO cauldronTemplate;
}