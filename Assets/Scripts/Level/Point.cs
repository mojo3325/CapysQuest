using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    private AudioSource audioSource;

    private void OnEnable()
    {
        EventManager.OnSoundChangeClick.AddListener(SoundTurn);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy")
        {
            audioSource.Play();
        }
    }

    private void SoundTurn(SoundState state)
    {
        audioSource.mute = (state == SoundState.Off) ? true : false ;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
