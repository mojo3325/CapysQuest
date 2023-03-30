using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MusicSource : MonoBehaviour
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
        CapyController.OnTimeLost += SetMusicVolumeQuite;
        MenuBar.PlayButtonClicked += SetStartMusic;
        ZoneController.OnZoneAchieved += OnZoneAchieved;
    }

    private void OnDisable()
    {
        SettingsController.SoundChanged -= SoundTurn;
        CapyCharacter.OnCapyDied -= (d, v) => SetMusicVolumeQuite();
        CapyController.OnTimeLost -= SetMusicVolumeQuite;
        MenuBar.PlayButtonClicked -= SetStartMusic;
        ZoneController.OnZoneAchieved -= OnZoneAchieved;
    }

    private void OnZoneAchieved(ZoneType type)
    {
        if (type == ZoneType.zone_3)
            _musicTranslateCoroutine = StartCoroutine(MusicTranslate());
    }

    private IEnumerator MusicTranslate()
    {
        SetMusicVolumeQuite();
        yield return new WaitForSeconds(2f);
        _audioSource.clip = continueMusic;
        _audioSource.Play();
        SetMusicVolumeDefault();
    }
    
    private void SetMusicVolumeQuite()
    {
        _audioSource.volume = 0.05f;
    }

    private void SetMusicVolumeDefault()
    {
        _audioSource.volume = 0.2f;
    }

    private void SetStartMusic()
    {
        _audioSource.clip = startMusic;
        _audioSource.Play();
        SetMusicVolumeDefault();
    }
}