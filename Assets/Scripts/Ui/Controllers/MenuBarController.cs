using System;
using UnityEngine;

public class MenuBarController : MonoBehaviour
{
    public static event Action<SoundState> SoundChanged;

    private SoundState soundState = SoundState.On;

    private void OnEnable()
    {
        MenuBar.SoundButtonClicked += ChangeSoundState;
    }

    private void ChangeSoundState()
    {
        soundState = (soundState == SoundState.On) ? SoundState.Off : SoundState.On;
        SoundChanged?.Invoke(soundState);
    }

    private void OnDisable()
    {
        MenuBar.SoundButtonClicked -= ChangeSoundState;
    }
}
