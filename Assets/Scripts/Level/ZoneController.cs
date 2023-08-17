using System;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public static event Action<Level> OnLevelAchieved;
    private bool _isChecked;

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += (it) => ResetZoneState();
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= (it) => ResetZoneState();
    }

    private void ResetZoneState()
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Capy") && _isChecked == false)
        {
            OnLevelAchieved?.Invoke(gameObject.GetComponent<LevelScript>().level);
            _isChecked = true;
        }
    }
}
