using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClipsSO audioClipsSO;

    private void Start()
    {
        Player.Instance.OnObjectPickup += Player_OnObjectPickup;
        BaseCounter.OnObjectDrop += BaseCounter_OnObjectDrop;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void Player_OnObjectPickup(object sender, System.EventArgs e)
    {
        PlayClips(audioClipsSO.objectPickup, Player.Instance.transform.position);
    }

    private void BaseCounter_OnObjectDrop(object sender, System.EventArgs e)
    {
        MonoBehaviour monoBehaviour = sender as MonoBehaviour;
        PlayClips(audioClipsSO.objectDrop, monoBehaviour.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        MonoBehaviour monoBehaviour = sender as MonoBehaviour;
        PlayClips(audioClipsSO.chop, monoBehaviour.transform.position);
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        PlayClips(audioClipsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
    }

    private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e)
    {
        PlayClips(audioClipsSO.deliveryFail, DeliveryCounter.Instance.transform.position);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        MonoBehaviour monoBehaviour = sender as MonoBehaviour;
        PlayClips(audioClipsSO.trash, monoBehaviour.transform.position);
    }

    private void PlayClips(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        PlayClip(clips[Random.Range(0, clips.Length)], position, volume);
    }

    private void PlayClip(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
