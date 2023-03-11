using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject meat1;
    [SerializeField] private GameObject meat2;
    [SerializeField] private GameObject meat3;
    [SerializeField] private GameObject meat4;
    [SerializeField] private GameObject meat5;
    [SerializeField] private GameObject menuText;
    [SerializeField] private GameObject TimeMenu;
    [SerializeField] private GameController gameController;
    private float timeCount;
    private bool isTimeOver;
    private bool isDead;

    private void FixedUpdate()
    {
        if (gameController != null)
        {
            timeCount = gameController.TimeCount;
            isTimeOver = gameController.IsTimeOver;
            isDead = gameController.IsDead;

            if (isDead == true)
            {
                TimeMenu.SetActive(false);
            }
            else
            {
                TimeMenu.SetActive(true);
                meat1.SetActive(timeCount >= 1);
                meat2.SetActive(timeCount >= 2);
                meat3.SetActive(timeCount >= 3);
                meat4.SetActive(timeCount >= 4);
                meat5.SetActive(timeCount >= 5);
            }

            if (isTimeOver == true)
            {
                menuText.SetActive(true);
                menuText.GetComponent<Text>().text = "Время вышло";
            }
            else
            {
                menuText.SetActive(false);
                menuText.GetComponent<Text>().text = "";
            }
        }
    }
}
