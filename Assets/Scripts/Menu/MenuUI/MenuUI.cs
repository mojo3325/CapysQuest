using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    [SerializeField] private GameObject SoundOffButton;
    [SerializeField] private GameObject SoundOnButton;
    [SerializeField] private GameController gameController;

    private bool isMute;
    private void FixedUpdate()
    {
        if (gameController != null)
        {
            isMute = gameController.IsMute;
            SoundOffButton.SetActive(isMute == false);
            SoundOnButton.SetActive(isMute == true);
        }
    }
}
