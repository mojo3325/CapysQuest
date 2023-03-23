using System;
using UnityEngine;

public class BoosterController : MonoBehaviour
{
    public static event Action OnBoosterClaimed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy") 
        {
            OnBoosterClaimed?.Invoke();
            gameObject.SetActiveRecursively(false);
        }
    }

    public void ResetState()
    {
        gameObject.SetActiveRecursively(true);
    }
}
