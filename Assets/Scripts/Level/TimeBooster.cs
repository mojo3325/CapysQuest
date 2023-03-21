using UnityEngine;

public class TimeBooster : MonoBehaviour
{
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
            gameObject.SetActive(false);
        }
    }
    
    private void ResetBoosterState()
    {
        gameObject.SetActive(true);
    }
}
