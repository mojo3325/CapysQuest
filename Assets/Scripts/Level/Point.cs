using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    private AudioSource audioSource;
    private bool _isChecked;

    private void OnEnable()
    {
        EventManager.OnSoundChangeClick.AddListener(SoundTurn);
        EventManager.OnCapyDie.AddListener(ResetPointState);
    }

    private void ResetPointState(DieType die)
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy" && _isChecked == false)
        {
            _isChecked = true;
            EventManager.OnPointReached.Invoke();
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
