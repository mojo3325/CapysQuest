using System;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public static event Action<ZoneType> OnZoneAchieved;
    private bool _isChecked;

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += ResetZoneState;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= ResetZoneState;
    }

    private void ResetZoneState()
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Capy") && _isChecked == false)
        {
            switch (gameObject.tag)
            {
                case "zone1":
                    OnZoneAchieved?.Invoke(ZoneType.zone_1);
                    _isChecked = true;
                    break;
                case "zone2":
                    OnZoneAchieved?.Invoke(ZoneType.zone_2);
                    _isChecked = true;
                    break;
                case "zone3":
                    OnZoneAchieved?.Invoke(ZoneType.zone_3);
                    _isChecked = true;
                    break;
                case "zone4":
                    OnZoneAchieved?.Invoke(ZoneType.zone_4);
                    _isChecked = true;
                    break;
                case "finish_zone":
                    OnZoneAchieved?.Invoke(ZoneType.zone_finish);
                    _isChecked = true;
                    break;
            }
        }
    }
}
