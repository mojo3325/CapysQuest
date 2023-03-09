using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject Flag2;
    [SerializeField] private GameObject Flag3;
    [SerializeField] private GameObject Flag4;
    [SerializeField] private GameObject FlagFinish;
    private bool _zone2Achieved;
    private bool _zone3Achieved;
    private bool _zone4Achieved;
    private bool _finishAchieved;

    void FixedUpdate()
    {
        _zone2Achieved = gameController.Zone2Achieved;
        _zone3Achieved = gameController.Zone3Achieved;
        _zone4Achieved = gameController.Zone4Achieved;
        _finishAchieved = gameController.FinishAchieved;

        if (_zone2Achieved)
        {
            Flag2.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Flag2.GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0.2f), Color.white, 0.2f);
        }

        if (_zone3Achieved)
        {
            Flag3.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Flag3.GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0.2f), Color.white, 0.2f);
        }

        if (_zone4Achieved)
        {
            Flag4.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Flag4.GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0.2f), Color.white, 0.2f);
        }

        if (_finishAchieved)
        {
            FlagFinish.GetComponent<Image>().color = Color.white;
        }
        else
        {
            FlagFinish.GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0.2f), Color.white, 0.2f);
        }
    }
}
