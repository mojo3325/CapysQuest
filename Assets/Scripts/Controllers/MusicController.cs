using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;
    
    [Header("Музыка")] 
    [SerializeField] private AudioClip startMusic;
    [SerializeField] private AudioClip continueMusic;

    private Coroutine _musicTranslateCoroutine;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state != SoundState.On);
    }

    private void OnEnable()
    {
        SettingsController.SoundChanged += SoundTurn;
        CapyCharacter.OnCapyDied += (d, v) => SetMusicVolumeQuite();
        MenuBar.PlayButtonClicked += (level) => SetStartMusic(level);
        IntAdController.AdShown += AudioMute;
        IntAdController.OnAdClosed += AudioUnMute;
    }

    private void AudioMute()
    {
        SoundTurn(SoundState.Off);
    }
    
    private void AudioUnMute()
    {
        SoundTurn(SoundState.On);
    }
    
    private void OnDisable()
    {
        SettingsController.SoundChanged -= SoundTurn;
        CapyCharacter.OnCapyDied -= (d, v) => SetMusicVolumeQuite();
        MenuBar.PlayButtonClicked -= (level) => SetStartMusic(level);
        IntAdController.AdShown -= AudioMute;
        IntAdController.OnAdClosed -= AudioUnMute;
    }

    private IEnumerator MusicTranslate()
    {
        SetMusicVolumeQuite();
        _audioSource.clip = continueMusic;
        _audioSource.Play();
        SetMusicVolumeDefault();
        yield return null;
    }
    
    private void SetMusicVolumeQuite()
    {
        _audioSource.volume = 0.1f;
    }

    private void SetMusicVolumeDefault()
    {
        _audioSource.volume = 0.5f;
    }

    private void SetStartMusic(Level level)
    {
        if (level >= Level.LEVEL6 && level <= Level.LEVEL10)
        {
            if(_musicTranslateCoroutine != null)
                StopCoroutine(_musicTranslateCoroutine);

            _musicTranslateCoroutine = StartCoroutine(MusicTranslate());
        }

        if (level >= Level.LEVEL1 &&level <= Level.LEVEL5)
            _audioSource.clip = startMusic;

        _audioSource.Play();
        SetMusicVolumeDefault();
    }
}