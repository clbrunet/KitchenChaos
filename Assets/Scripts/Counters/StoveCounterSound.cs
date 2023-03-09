using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private bool isBurning;
    private float elapsed;
    [SerializeField] private float warningInterval = 0.33f;

    private void Awake()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        stoveCounter.OnTurningOn += StoveCounter_OnTurningOn;
        stoveCounter.OnBurningStarted += StoveCounter_OnBurningStarted;
        stoveCounter.OnTurningOff += StoveCounter_OnTurningOff;
    }

    private void OnDisable()
    {
        stoveCounter.OnTurningOn -= StoveCounter_OnTurningOn;
        stoveCounter.OnBurningStarted -= StoveCounter_OnBurningStarted;
        stoveCounter.OnTurningOff -= StoveCounter_OnTurningOff;
    }

    private void Start()
    {
        audioSource.volume = SoundManager.Instance.GetVolume();
        SoundManager.Instance.OnSoundEffectsVolumeChanged += SoundManager_OnSoundEffectsVolumeChanged;
    }

    private void Update()
    {
        if (!isBurning)
        {
            return;
        }
        elapsed += Time.deltaTime;
        if (elapsed > warningInterval)
        {
            SoundManager.Instance.PlayWarning(transform.position);
            elapsed = 0f;
        }
    }

    private void SoundManager_OnSoundEffectsVolumeChanged(object sender, SoundManager.OnSoundEffectsVolumeChangedEventArgs e)
    {
        audioSource.volume = e.volume;
    }

    private void StoveCounter_OnTurningOn(object sender, System.EventArgs e)
    {
        audioSource.Play();
    }

    private void StoveCounter_OnBurningStarted(object sender, System.EventArgs e)
    {
        isBurning = true;
        elapsed = 0f;
    }

    private void StoveCounter_OnTurningOff(object sender, System.EventArgs e)
    {
        audioSource.Pause();
        isBurning = false;
    }
}
