using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    private AudioSource audioSource;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of MusicManager");
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);
    }

    public void ChangeVolume()
    {
        if (audioSource.volume >= 1f)
        {
            audioSource.volume = 0f;
        }
        else
        {
            audioSource.volume += 0.1f;
            if (audioSource.volume > 1f)
            {
                audioSource.volume = 1f;
            }
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, audioSource.volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }
}
