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

    private static string _languageLabelName = "LanguageText";
    private static string _languageButtonName = "LanguageButton";
    private static string _backButtonName = "BackButton";

    [SerializeField] private Sprite russianLang;
    [SerializeField] private Sprite englishLang;
    [SerializeField] private Sprite ukrainianLang;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _languageLabel = _root.Q<Label>(_languageLabelName);
        _languageButton = _root.Q<Button>(_languageButtonName);
        _backButton = _root.Q<Button>(_backButtonName);
    }

    private void OnEnable()
    {
        SettingsController.LanguageChanged += SetupLanguageStatus;
        MenuManagerController.ConnectionIsChecked += _mainMenuUIManager.CheckConnection;
    }

    private void OnDisable()
    {
        SettingsController.LanguageChanged -= SetupLanguageStatus;
        MenuManagerController.ConnectionIsChecked -= _mainMenuUIManager.CheckConnection;
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
     }

    private void OnSettingsBackClicked()
    {
        if (ScreenBefore == ScreenBefore.HomeScreen || ScreenBefore == ScreenBefore.Null)
            _mainMenuUIManager.ShowHomeScreen();
        if (ScreenBefore == ScreenBefore.GameOver)
            StartCoroutine(_mainMenuUIManager.ShowGameOverAfter(0f));

        ScreenBefore = ScreenBefore.Null;
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
