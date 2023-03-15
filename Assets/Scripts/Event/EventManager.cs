using UnityEngine;
using UnityEngine.Events;


public enum SoundState
{
    On, Off
}

public enum DieType
{
    Enemy, River, Timer
}

public enum ZoneType
{
    zone_1, zone_2, zone_3, zone_4
}

public class EventManager : MonoBehaviour
{
    public static UnityEvent OnPlayClick = new UnityEvent();
    public static UnityEvent<DieType> OnCapyDie = new UnityEvent<DieType>();
    public static UnityEvent OnJetpackClaimed = new UnityEvent();
    public static UnityEvent OnTimeClimed = new UnityEvent();
    public static UnityEvent<SoundState> OnSoundChangeClick = new UnityEvent<SoundState>();
    public static UnityEvent OnRightTap = new UnityEvent();
    public static UnityEvent OnLeftTap = new UnityEvent();
    public static UnityEvent<float> OnTimeChange = new UnityEvent<float>();
    public static UnityEvent OnTimeLost = new UnityEvent();
    public static UnityEvent<ZoneType> OnZoneAchieved = new UnityEvent<ZoneType>();
    public static UnityEvent OnFinishZoneAchieved = new UnityEvent();
    public static UnityEvent OnLanguageChange = new UnityEvent();
    public static UnityEvent OnBoosterPick = new UnityEvent();
    public static UnityEvent OnTutorialAccept = new UnityEvent();
}
