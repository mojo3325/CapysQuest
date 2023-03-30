using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MenuScreen
{
    public static event Action IsShown;

    public static event Action LanguageButtonClicked;

    private Label _languageLabel;
    private Button _languageButton;
    private Button _backButton;
    private Button _privacyButton;

    private static string _languageLabelName = "LanguageText";
    private static string _languageButtonName = "LanguageButton";
    private static string _backButtonName = "BackButton";
    private static string _privacyButtonName = "PrivacyPolicyButton";

    [SerializeField] private Sprite russianLang;
    [SerializeField] private Sprite englishLang;
    [SerializeField] private Sprite ukrainianLang;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _languageLabel = _root.Q<Label>(_languageLabelName);
        _languageButton = _root.Q<Button>(_languageButtonName);
        _backButton = _root.Q<Button>(_backButtonName);
        _privacyButton = _root.Q<Button>(_privacyButtonName);

    }

    private void OnEnable()
    {
        SettingsController.LanguageChanged += SetupLanguageStatus;
    }

    private void OnDisable()
    {
        SettingsController.LanguageChanged -= SetupLanguageStatus;
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
        SetupLanguageStatus();
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _languageButton.clicked += () => LanguageButtonClicked?.Invoke();
        _backButton.clicked += () => OnSettingsBackClicked();
        _privacyButton.clicked += () => OpenPrivacyPolicySite();
     }

    private void OpenPrivacyPolicySite()
    {
        Application.OpenURL("https://sites.google.com/view/capys-quest-privacy-policy/home");
    }

    private void OnSettingsBackClicked()
    {
        if (ScreenBefore is HomeScreen || ScreenBefore == null)
            _mainMenuUIManager.ShowHomeScreen();
        if (ScreenBefore is GameOverScreen)
            StartCoroutine(_mainMenuUIManager.ShowGameOverAfter(0f));
    }

    private void SetupLanguageStatus()
    {
        LocalizationManager.Read();

        var language = LocalizationManager.Language;

        _languageLabel.text = LocalizationManager.Localize("game_language");

        switch (language)
        {
            case "English":
                _languageButton.style.backgroundImage = new StyleBackground(englishLang);
                break;

            case "Ukrainian":
                _languageButton.style.backgroundImage = new StyleBackground(ukrainianLang);
                break;

            case "Russian":
                _languageButton.style.backgroundImage = new StyleBackground(russianLang);
                break;
        }
    }
}
