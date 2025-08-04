using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public SoundManager soundManager;
    public MusicManager musicManager;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider overallSlider;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float overralVolume = 1f;

    [SerializeField]
    private string sceneName;

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

    private void Start()
    {
        MusicManager.Instance.PlayMusic("MainMenu");
        SaveVolume(1f, 1f, 1f);
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
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        overralVolume = PlayerPrefs.GetFloat("OverallVolume");

        Debug.Log($"Loaded Music Volume: {musicVolume}, SFX Volume: {sfxVolume}, Overall Volume: {overralVolume}");
    }
}
