using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void SoundTurn(SoundState state)
    {
        audioSource.mute = (state == SoundState.On) ? false : true;
    }

    private void OnEnable()
    {
        EventManager.OnSoundChangeClick.AddListener(SoundTurn);
    }
}
