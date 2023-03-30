using System;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] private AudioClip _zoneProgressSound;
    [SerializeField] private AudioClip _timePointSound;

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
        if (other.gameObject.tag == "Capy" && _isChecked == false && gameObject.tag != "point")
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
        else if(other.gameObject.tag == "Capy" && _isChecked == false && gameObject.tag == "point")
        {
            OnZoneAchieved?.Invoke(ZoneType.time_booster);
            _isChecked = true;
            _audioSource.PlayOneShot(_timePointSound);
        }
    }

    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state == SoundState.On) ? false : true;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
}
