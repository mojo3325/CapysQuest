using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;
public class SettingsController : MonoBehaviour
{
    public static event Action LanguageChanged;

    private void OnEnable()
    {
        SettingsScreen.LanguageButtonClicked += ChangeGameLanguage;
    }

    private void OnDisable()
    {
        SettingsScreen.LanguageButtonClicked -= ChangeGameLanguage;
    }

    private void ChangeGameLanguage()
    {
        var language = PlayerPrefs.GetString("game_language", "");

        if (language != "")
        {
            if (language == "English")
            {
                saveSelectedLanguage("Ukrainian");
            }
            else if (language == "Ukrainian")
            {
                saveSelectedLanguage("Russian");
            }
            else if (language == "Russian")
            {
                saveSelectedLanguage("English");
            }
        }
        else
        {
            var sysLanguage = Application.systemLanguage;
            if (sysLanguage == SystemLanguage.English)
            {
                saveSelectedLanguage("English");
            }
            else if (sysLanguage == SystemLanguage.Russian)
            {
                saveSelectedLanguage("Russian");
            }
            else if (sysLanguage == SystemLanguage.Ukrainian)
            {
                saveSelectedLanguage("Ukrainian");
            }
            else
            {
                saveSelectedLanguage("Russian");
            }
        }
    }

    private void saveSelectedLanguage(string language)
    {
        LocalizationManager.Language = language;
        PlayerPrefs.SetString("game_language", language);
        LanguageChanged?.Invoke();
    }
}
