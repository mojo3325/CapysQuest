using Assets.SimpleLocalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuManager : MonoBehaviour
{
    private Label languageText;
    private Button languageButton;

    [SerializeField] private Sprite russianLang;
    [SerializeField] private Sprite englishLang;
    [SerializeField] private Sprite ukrainianLang;


    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        languageText = root.Q<Label>("LanguageText");
        languageButton = root.Q<Button>("LanguageButton");

        LocalizationManager.Read();

        SetupLanguageStatus();
        languageButton.clicked += () => ChangeGameLanguage();
    }

    private void ChangeGameLanguage()
    {
        var language = PlayerPrefs.GetString("game_language", "");

        if(language != "") 
        {
            if (language == "English")
            {
                saveSelectedLanguage("Ukranian");
            }
            else if (language == "Ukranian")
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
                saveSelectedLanguage("Ukranian");
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
        EventManager.OnLanguageChange.Invoke();
        SetupLanguageStatus();
    }

    private void SetupLanguageStatus()
    {
        var language = LocalizationManager.Language;

        languageText.text = LocalizationManager.Localize("game_language");

        switch (language)
        {
            case "English":
                languageButton.style.backgroundImage = new StyleBackground(englishLang);
                break;

            case "Ukranian":
                languageButton.style.backgroundImage = new StyleBackground(ukrainianLang);
                break;

            case "Russian":
                languageButton.style.backgroundImage = new StyleBackground(russianLang);
                break;
        }
    }
}
