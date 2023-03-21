using System;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    private AudioSource audioSource;
    private bool _isChecked;

    private void OnEnable()
    {
        MenuBarController.SoundChanged += SoundTurn;
        MenuBar.PlayButtonClicked += ResetZoneState;
    }

    private void OnDisable()
    {
        MenuBarController.SoundChanged -= SoundTurn;
        MenuBar.PlayButtonClicked -= ResetZoneState;
    }

    private void ResetZoneState()
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy" && _isChecked == false)
        {

            _isChecked = true;
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
