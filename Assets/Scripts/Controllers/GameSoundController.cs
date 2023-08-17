using System;
using UnityEngine;

public class GameSoundController : MonoBehaviour
{
    [Header("Звуки Кэпи")]
    [SerializeField] private AudioClip _boosterPickSound;
    [SerializeField] private AudioClip _helmetLoseSound;
    [SerializeField] private AudioClip _jetpackSound;
    [SerializeField] private AudioClip _jumperSound;
    [SerializeField] private AudioClip _capyJumpSound;
    [SerializeField] private AudioClip _defaultDieSound;
    [SerializeField] private AudioClip _waterDieSound;
    [SerializeField] private AudioClip _zoneProgressSound;

    [Header("Звук")]
    [SerializeField] private SoundState soundState;
    
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayBoosterPickSound()
    {
        _audioSource.PlayOneShot(_boosterPickSound);
    }

    public void PlayHelmetLoseSound()
    {
        _audioSource.PlayOneShot(_helmetLoseSound);
    }

    public void PlayJetpackSound()
    {
        _audioSource.PlayOneShot(_jetpackSound);
    }

    public void PlayJumperSound()
    {
        _audioSource.PlayOneShot(_jumperSound);
    }

    public void PlayCapyJumpSound()
    {
        _audioSource.PlayOneShot(_capyJumpSound);
    }
    
    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state != SoundState.On);
    }

    private void OnEnable()
    {
        SettingsController.SoundChanged += SoundTurn;
        CapyCharacter.OnCapyDied += OnCapyDie;
        ZoneController.OnLevelAchieved += PlayZoneAchieveSound;
    }
    
    private void OnDisable()
    {
        SettingsController.SoundChanged -= SoundTurn;
        CapyCharacter.OnCapyDied -= OnCapyDie;
        ZoneController.OnLevelAchieved -= PlayZoneAchieveSound;
    }

    private void PlayZoneAchieveSound(Level level)
    {
        _audioSource.PlayOneShot(_zoneProgressSound);
    }
    
    private void PlayDefaultDieSound()
    {
        _audioSource.PlayOneShot(_defaultDieSound);
    }

    private void PlayWaterDieSound()
    {
        _audioSource.PlayOneShot(_waterDieSound);
    }
    
    private void OnCapyDie(DieType dieType, Vector3 position)
    {
        if (dieType == DieType.Enemy)
            PlayDefaultDieSound();
        if (dieType == DieType.River)
            PlayWaterDieSound();
    }

}