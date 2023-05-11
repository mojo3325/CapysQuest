using System;
using UnityEngine;

public class MenuSoundController : MonoBehaviour
{
    private AudioSource _audioSource;
    
    [Header("Звуки")] 
    [SerializeField] private AudioClip MenuOpen;
    [SerializeField] private AudioClip MenuClose;
    [SerializeField] private AudioClip MenuEnter;

    private Coroutine _musicTranslateCoroutine;
    
    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state != SoundState.On);
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void PlayOpenSound()
    {
        _audioSource.Stop();
        _audioSource.clip = MenuOpen;
        _audioSource.Play();
    }
    
    private void PlayCloseSound()
    {
        _audioSource.Stop();
        _audioSource.clip = MenuClose;
        _audioSource.Play();   
    }
    
    private void PlayEnterSound()
    {
        _audioSource.Stop();
        _audioSource.clip = MenuEnter;
        _audioSource.Play();
    }

    private void OnEnable()
    {
        ButtonEvent.CloseMenuCalled += PlayCloseSound;
        ButtonEvent.EnterButtonCalled += PlayEnterSound;
        ButtonEvent.OpenMenuCalled += PlayOpenSound;
        SettingsController.SoundChanged += SoundTurn;
    }

    private void OnDisable()
    {
        ButtonEvent.CloseMenuCalled -= PlayCloseSound;
        ButtonEvent.EnterButtonCalled -= PlayEnterSound;
        ButtonEvent.OpenMenuCalled -= PlayOpenSound;
        SettingsController.SoundChanged -= SoundTurn;
    }
}
