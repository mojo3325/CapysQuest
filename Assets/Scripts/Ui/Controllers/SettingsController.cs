using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;
public class SettingsController : MonoBehaviour
{
    public static event Action LanguageChanged;
    
    public static event Action<SoundState> SoundChanged;

    private SoundState _soundState = SoundState.On;

    private void ChangeSoundState()
    {
        _soundState = (_soundState == SoundState.On) ? SoundState.Off : SoundState.On;
        SoundChanged?.Invoke(_soundState);
    }

    private void OnEnable()
    {
        SettingsScreen.LanguageButtonClicked += ChangeGameLanguage;
        SettingsScreen.SoundButtonClicked += ChangeSoundState;
    }

    private void OnDisable()
    {
        SettingsScreen.LanguageButtonClicked -= ChangeGameLanguage;
        SettingsScreen.SoundButtonClicked -= ChangeSoundState;
    }

    private void ChangeGameLanguage()
    {
        var language = PlayerPrefs.GetString("game_language", "");

        if (language != "")
        {
            if (language == "English")
            {
                SaveSelectedLanguage("Ukrainian");
            }
            else if (language == "Ukrainian")
            {
                SaveSelectedLanguage("Russian");
            }
            else if (language == "Russian")
            {
                SaveSelectedLanguage("English");
            }
        }
        else
        {
            var sysLanguage = Application.systemLanguage;
            switch (sysLanguage)
            {
                case SystemLanguage.English:
                    SaveSelectedLanguage("English");
                    break;
                case SystemLanguage.Russian:
                    SaveSelectedLanguage("Russian");
                    break;
                case SystemLanguage.Ukrainian:
                    SaveSelectedLanguage("Ukrainian");
                    break;
                default:
                    SaveSelectedLanguage("Russian");
                    break;
            }
        }
    }

    private void SaveSelectedLanguage(string language)
    {
        LocalizationManager.Language = language;
        PlayerPrefs.SetString("game_language", language);
        LanguageChanged?.Invoke();
    }
}
