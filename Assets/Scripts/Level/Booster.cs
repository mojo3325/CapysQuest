using UnityEngine;

public class Booster : MonoBehaviour
{

    private void OnEnable()
    {
        EventManager.OnPlayClick.AddListener(ResetBoosterState);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy") 
        {
            EventManager.OnBoosterPick.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void ResetBoosterState()
    {
        gameObject.SetActive(true);
    }
}
