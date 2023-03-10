using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    [SerializeField] private AudioClipsSO audioClipsSO;

    private float volume;
    public event EventHandler<OnSoundEffectsVolumeChangedEventArgs> OnSoundEffectsVolumeChanged;
    public class OnSoundEffectsVolumeChangedEventArgs : EventArgs
    {
        public float volume;

        public OnSoundEffectsVolumeChangedEventArgs(float volume)
        {
            this.volume = volume;
        }
    }

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of SoundManager");
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start()
    {
        //Player.Instance.OnObjectPickup += Player_OnObjectPickup;
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
    }

    private void OnEnable()
    {
        BaseCounter.OnObjectDrop += BaseCounter_OnObjectDrop;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void OnDisable()
    {
        BaseCounter.OnObjectDrop -= BaseCounter_OnObjectDrop;
        CuttingCounter.OnAnyCut -= CuttingCounter_OnAnyCut;
        TrashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed;
    }

    public float GetVolume()
    {
        return volume;
    }

    public void ChangeVolume()
    {
        if (volume >= 1f)
        {
            volume = 0f;
        }
        else
        {
            volume += 0.1f;
            if (volume > 1f)
            {
                volume = 1f;
            }
        }
        OnSoundEffectsVolumeChanged?.Invoke(this, new OnSoundEffectsVolumeChangedEventArgs(volume));
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    private void Player_OnObjectPickup(object sender, System.EventArgs e)
    {
        //PlaySound(audioClipsSO.objectPickup, Player.Instance.transform.position);
    }

    private void BaseCounter_OnObjectDrop(object sender, System.EventArgs e)
    {
        MonoBehaviour monoBehaviour = sender as MonoBehaviour;
        PlaySound(audioClipsSO.objectDrop, monoBehaviour.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        MonoBehaviour monoBehaviour = sender as MonoBehaviour;
        PlaySound(audioClipsSO.chop, monoBehaviour.transform.position);
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        PlaySound(audioClipsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
    }

    private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e)
    {
        PlaySound(audioClipsSO.deliveryFail, DeliveryCounter.Instance.transform.position);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        MonoBehaviour monoBehaviour = sender as MonoBehaviour;
        PlaySound(audioClipsSO.trash, monoBehaviour.transform.position);
    }

    public void PlayWarning(Vector3 position, float volumeMultiplier = 1f)
    {
        PlaySound(audioClipsSO.warning, position, volumeMultiplier);
    }

    public void PlaySound(AudioClip[] clips, Vector3 position, float volumeMultiplier = 1f)
    {
        PlaySound(clips[UnityEngine.Random.Range(0, clips.Length)], position, volumeMultiplier);
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume * volumeMultiplier);
    }
}
