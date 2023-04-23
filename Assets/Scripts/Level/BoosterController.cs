using System;
using UnityEngine;

public class BoosterController : MonoBehaviour
{
    public static event Action OnBoosterClaimed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Capy")) 
        {
            OnBoosterClaimed?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
    }
}
