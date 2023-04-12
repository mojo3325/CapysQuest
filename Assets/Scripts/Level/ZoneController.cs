using System;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] private AudioClip _zoneProgressSound;

    public static event Action<ZoneType> OnZoneAchieved;

    private AudioSource _audioSource;
    private bool _isChecked;

    private void OnEnable()
    {
        SettingsController.SoundChanged += SoundTurn;
        MenuBar.PlayButtonClicked += ResetZoneState;
    }

    private void OnDisable()
    {
        SettingsController.SoundChanged -= SoundTurn;
        MenuBar.PlayButtonClicked -= ResetZoneState;
    }

    private void ResetZoneState()
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy" && _isChecked == false && gameObject.tag != "finish_zone")
        {
            switch (gameObject.tag)
            {
                case "zone1":
                    OnZoneAchieved?.Invoke(ZoneType.zone_1);
                    break;
                case "zone2":
                    OnZoneAchieved?.Invoke(ZoneType.zone_2);
                    break;
                case "zone3":
                    OnZoneAchieved?.Invoke(ZoneType.zone_3);
                    break;
                case "zone4":
                    OnZoneAchieved?.Invoke(ZoneType.zone_4);
                    break;
            }
            _isChecked = true;
            _audioSource.PlayOneShot(_zoneProgressSound);
        }
        if(other.gameObject.tag == "Capy" && _isChecked == false && gameObject.tag == "finish_zone")
        {
            OnZoneAchieved?.Invoke(ZoneType.zone_finish);
            _isChecked = true;
            _audioSource.PlayOneShot(_zoneProgressSound);
        }
    }

    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state != SoundState.On);
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
}
