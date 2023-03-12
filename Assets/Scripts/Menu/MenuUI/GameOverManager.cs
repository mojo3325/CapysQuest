using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameOverMamager : MonoBehaviour
{
    private VisualElement Zone1;
    private VisualElement Zone2;
    private VisualElement Zone3;
    private VisualElement Zone4;
    private VisualElement FinishZone;

    private bool _zone1Achieved;
    private bool _zone2Achieved;
    private bool _zone3Achieved;
    private bool _zone4Achieved;
    private bool _finishAchieved;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        Zone1 = root.Q<VisualElement>("Zone1");
        Zone2 = root.Q<VisualElement>("Zone2");
        Zone3 = root.Q<VisualElement>("Zone3");
        Zone4 = root.Q<VisualElement>("Zone4");
        FinishZone = root.Q<VisualElement>("FinishZone");

        EventManager.OnZone1Achieved.AddListener(SetZone1Achieved);
        EventManager.OnZone2Achieved.AddListener(SetZone2Achieved);
        EventManager.OnZone3Achieved.AddListener(SetZone3Achieved);
        EventManager.OnZone4Achieved.AddListener(SetZone4Achieved);
        EventManager.OnFinishZoneAchieved.AddListener(SetFinihsZoneAchieved);
        EventManager.OnCapyDie.AddListener(ShowUserProgess);
        EventManager.OnPlayClick.AddListener(ResetUserProgress);
    }

    private void ResetUserProgress()
    {
        Debug.Log("ResetUserProgess");
        _zone1Achieved = false;
        _zone2Achieved = false;
        _zone3Achieved = false;
        _zone4Achieved = false;
        _finishAchieved = false;
    }

    private void ShowUserProgess(DieType dieType)
    {
        StartCoroutine(ShowProgressAfterTime());
    }

    IEnumerator ShowProgressAfterTime()
    {
        yield return new WaitForSeconds(1.5f);
        
        if(_zone1Achieved)
            Zone1.style.opacity = 1f;
        if(_zone2Achieved)
            Zone2.style.opacity = 1f;
        if (_zone3Achieved)
            Zone3.style.opacity = 1f;
        if (_zone4Achieved)
            Zone4.style.opacity = 1f;
        if (_finishAchieved)
            FinishZone.style.opacity = 1f;
    }

    private void SetZone1Achieved()
    {
        _zone1Achieved = true;
    }

    private void SetZone2Achieved()
    {
        _zone2Achieved = true;
    }

    private void SetZone3Achieved()
    {
        _zone3Achieved = true;
    }

    private void SetZone4Achieved()
    {
        _zone4Achieved = true;
    }

    private void SetFinihsZoneAchieved()
    {
        _finishAchieved = true;
    }
}
