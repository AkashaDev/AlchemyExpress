using System.Collections.Generic;
using AlchemyExpress.Quest;
using UnityEngine;
using UnityEngine.Localization; // 1. Tambahkan namespace ini

[CreateAssetMenu(fileName = "Quest_", menuName = "Alchemy/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Detail Misi")]
    [Tooltip("Pilih dialog dari String Table. Gunakan <b>kalimat</b> di dalam tabel untuk membuat teks tebal.")]
    // 2. Ubah tipe data string menjadi LocalizedString. 
    // Atribut [TextArea] bisa dihapus karena teks akan diedit di String Table, bukan langsung di SO.
    public LocalizedString dialogue; 
    
    public Potion Potion;
    public LevelDifficult levelDifficults;

    [Header("Reward Berdasarkan Mood")]
    public int rewardHappy;
    public int rewardNeutral;
    public int rewardAngry;

    [Header("Pengaturan Gameplay")]
    public CauldronTemplateSO cauldronTemplate;
}