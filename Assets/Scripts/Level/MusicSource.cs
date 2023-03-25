using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state == SoundState.On) ? false : true;
    }

    private void OnEnable()
    {
        MenuBarController.SoundChanged += SoundTurn;
        CapyCharacter.OnCapyDied += MusicQuite;
        MenuBar.PlayButtonClicked += MusicDefault;
    }

    private void MusicQuite(DieType D, Vector3 V)
    {
        _audioSource.volume = 0.05f;
    }

    private void MusicDefault()
    {
        _audioSource.volume = 0.2f;
    }

    private void OnDisable()
    {
        MenuBarController.SoundChanged -= SoundTurn;
        CapyCharacter.OnCapyDied -= MusicQuite;
        MenuBar.PlayButtonClicked -= MusicDefault;
    }
}
