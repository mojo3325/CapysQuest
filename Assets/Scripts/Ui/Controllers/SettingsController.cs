using System;
using UnityEngine;
using UnityEngine.UIElements;
public class SettingsController : MonoBehaviour
{
    public static event Action<SoundState> SoundChanged;

    private SoundState _soundState = SoundState.On;

    private void ChangeSoundState()
    {
        _soundState = (_soundState == SoundState.On) ? SoundState.Off : SoundState.On;
        SoundChanged?.Invoke(_soundState);
    }

    private void OnEnable()
    {
        SettingsScreen.SoundButtonClicked += ChangeSoundState;
    }

    private void OnDisable()
    {
        SettingsScreen.SoundButtonClicked -= ChangeSoundState;
    }
}
