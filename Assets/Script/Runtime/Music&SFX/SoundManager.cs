using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DLibrary;

    private void Awake()
    {
        if (Instance != null)
        {
            //
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DLibrary.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
    }
    
    public float SFXVolume
    {
        get => sfx2DLibrary.volume;
        set => sfx2DLibrary.volume = value;
    }
}
