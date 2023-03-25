using System;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] private AudioClip _zoneProgressSound;
    [SerializeField] private AudioClip _timePointSound;

    public static event Action<ZoneType> OnZoneAchieved;

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
            audioSource.PlayOneShot(_zoneProgressSound);
        }
        else if(other.gameObject.tag == "Capy" && _isChecked == false && gameObject.tag == "point")
        {
            OnZoneAchieved?.Invoke(ZoneType.time_booster);
            _isChecked = true;
            audioSource.PlayOneShot(_timePointSound);
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
