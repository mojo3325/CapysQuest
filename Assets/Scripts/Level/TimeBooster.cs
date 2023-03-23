using UnityEngine;

public class TimeBooster : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy")
        {
            gameObject.SetActiveRecursively(false);
        }
    }

    public void ResetState()
    {
        gameObject.SetActiveRecursively(true);
    }

}
