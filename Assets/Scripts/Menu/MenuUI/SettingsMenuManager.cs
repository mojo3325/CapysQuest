using Assets.SimpleLocalization;
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
        var language = LocalizationManager.Language;

        switch (language)
        {
            case "English":
                LocalizationManager.Language = "Ukranian";
                SetupLanguageStatus();
                PlayerPrefs.SetString("game_language", "Ukranian");
                EventManager.OnLanguageChange.Invoke();
                break;
            case "Ukranian":
                LocalizationManager.Language = "Russian";
                SetupLanguageStatus();
                PlayerPrefs.SetString("game_language", "Russian");
                EventManager.OnLanguageChange.Invoke();
                break;
            case "Russian":
                LocalizationManager.Language = "English";
                SetupLanguageStatus();
                PlayerPrefs.SetString("game_language", "English");
                EventManager.OnLanguageChange.Invoke();
                break;
        }
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
