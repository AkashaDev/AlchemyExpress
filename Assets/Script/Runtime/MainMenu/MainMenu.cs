using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Localization.Settings; // 1. Namespace untuk Localization

public class MainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public SoundManager soundManager;
    public MusicManager musicManager;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider overallSlider;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float overralVolume = 1f;

    [Header("Scene Settings")]
    [SerializeField]
    private string sceneName;

    [Header("Language Settings")]
    private bool isChangingLanguage = false;
    private const string LanguageKey = "SavedLanguageIndex"; // Kunci untuk simpan bahasa

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = FindObjectOfType<SoundManager>();
        }
        if (musicManager == null)
        {
            musicManager = FindObjectOfType<MusicManager>();
        }

        LoadVolume();
        UpdateOverallVolume(overralVolume);

        if (sfxSlider != null && musicSlider != null)
        {
            sfxSlider.value = sfxVolume;
            musicSlider.value = musicVolume;
            overallSlider.value = overralVolume;

            sfxSlider.onValueChanged.AddListener(UpdateSoundVolume);
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
            overallSlider.onValueChanged.AddListener(UpdateOverallVolume);
        }
    }

    // 2. Start diubah menjadi IEnumerator agar bisa menunggu sistem lokalisasi siap
    private IEnumerator Start()
    {
        MusicManager.Instance.PlayMusic("MainMenu");

        // Tunggu sistem lokalisasi selesai inisialisasi
        yield return LocalizationSettings.InitializationOperation;

        // Muat bahasa yang tersimpan (default ke 0 jika belum ada)
        int savedIndex = PlayerPrefs.GetInt(LanguageKey, 0);
        ApplyLanguage(savedIndex);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(sceneName);
        MusicManager.Instance.PlayMusic("Game");
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("game sudah diexit!");
    }

    // --- AUDIO SYSTEM ---

    public void UpdateMusicVolume(float volume)
    {
        musicVolume = volume;
        if (MusicManager.Instance != null)
            MusicManager.Instance.SFXVMusic = volume * overralVolume;

        SaveVolume(musicVolume, sfxVolume, overralVolume);
    }

    public void UpdateSoundVolume(float volume)
    {
        sfxVolume = volume;
        if (SoundManager.Instance != null)
            SoundManager.Instance.SFXVolume = volume * overralVolume;

        SaveVolume(musicVolume, sfxVolume, overralVolume);
    }

    public void UpdateOverallVolume(float volume)
    {
        overralVolume = volume;
        if (MusicManager.Instance != null)
            MusicManager.Instance.SFXVMusic = musicVolume * overralVolume;
        if (SoundManager.Instance != null)
            SoundManager.Instance.SFXVolume = sfxVolume * overralVolume;

        SaveVolume(musicVolume, sfxVolume, overralVolume);
    }

    public void SaveVolume(float musicVolume, float sfxVolume, float overalVolume)
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("OverallVolume", overalVolume);
        
        Debug.Log($"Loaded Music Volume: {musicVolume}, SFX Volume: {sfxVolume}, Overall Volume: {overralVolume}");
    }

    public void LoadVolume()
    {
        // 3. Menambahkan parameter nilai default '1f' agar suara tidak 0 pada instalasi pertama
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        overralVolume = PlayerPrefs.GetFloat("OverallVolume", 1f);

        Debug.Log($"Loaded Music Volume: {musicVolume}, SFX Volume: {sfxVolume}, Overall Volume: {overralVolume}");
    }

    // --- LANGUAGE SYSTEM ---

    public void ChangeLanguage(int localeIndex)
    {
        if (isChangingLanguage) return;
        StartCoroutine(SetLocaleAndSave(localeIndex));
    }

    private IEnumerator SetLocaleAndSave(int localeIndex)
    {
        isChangingLanguage = true;

        yield return LocalizationSettings.InitializationOperation;

        if (ApplyLanguage(localeIndex))
        {
            PlayerPrefs.SetInt(LanguageKey, localeIndex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Index bahasa tidak valid!");
        }

        isChangingLanguage = false;
    }

    private bool ApplyLanguage(int localeIndex)
    {
        if (localeIndex >= 0 && localeIndex < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
            return true;
        }
        return false;
    }
}