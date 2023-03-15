using UnityEngine;

public class TimeBooster : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnCapyDie.AddListener(ResetBoosterState);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy")
        {
            gameObject.SetActive(false);
        }
    }
    
    private void ResetBoosterState(DieType dieType)
    {
        gameObject.SetActive(true);
    }
}
