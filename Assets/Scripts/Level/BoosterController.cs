using System;
using UnityEngine;

public class BoosterController : MonoBehaviour
{
    public static event Action OnBoosterClaimed;
    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += ResetBoosterState;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= ResetBoosterState;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy") 
        {
            OnBoosterClaimed?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void ResetBoosterState()
    {
        gameObject.SetActive(true);
    }
}
