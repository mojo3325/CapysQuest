using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static UnityEvent OnPlayClick = new UnityEvent();
    public static UnityEvent OnChangeLanguageClick = new UnityEvent();
    public static UnityEvent<DieType> OnCapyDie = new UnityEvent<DieType>();
    public static UnityEvent OnPointReached = new UnityEvent();
    public static UnityEvent OnJetpackClaimed = new UnityEvent();
    public static UnityEvent<int> OnTimeClimed = new UnityEvent<int>();
    public static UnityEvent OnSoundMuteClick = new UnityEvent();
    public static UnityEvent OnSoundUnmuteClick = new UnityEvent();
    public static UnityEvent OnRightTap = new UnityEvent();
    public static UnityEvent OnLeftTap = new UnityEvent();
    public static UnityEvent<float> OnTimeChange = new UnityEvent<float>();
    public static UnityEvent OnTimeLost = new UnityEvent();
    public static UnityEvent OnZone1Achieved = new UnityEvent();
    public static UnityEvent OnZone2Achieved = new UnityEvent();
    public static UnityEvent OnZone3Achieved = new UnityEvent();
    public static UnityEvent OnZone4Achieved = new UnityEvent();
    public static UnityEvent OnFinishZoneAchieved = new UnityEvent();
}
