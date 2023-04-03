using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MenuScreen
{
    public static event Action IsShown;
    public static event Action LanguageButtonClicked;
    public static event Action SoundButtonClicked;

    [SerializeField]
    private Sprite SoundOn;
    [SerializeField]
    private Sprite SoundOff;

    private Label _languageLabel;
    private Label _soundLabel;
    
    private Button _languageButton;
    private Button _backButton;
    private Button _privacyButton;
    private Button _soundButton;
    private Button _referralButton;


    private static string _languageLabelName = "LanguageText";
    private static string _languageButtonName = "LanguageButton";
    private static string _backButtonName = "BackButton";
    private static string _privacyButtonName = "PrivacyPolicyButton";
    private static string _soundButtonName = "SoundButton";
    private static string _soundLabelName = "SoundText";
    private static string _referralButtonName = "ReferralButton";

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
        _soundButton = _root.Q<Button>(_soundButtonName);
        _soundLabel = _root.Q<Label>(_soundLabelName);
        _referralButton = _root.Q<Button>(_referralButtonName);

    }

    private void OnEnable()
    {
        SettingsController.LanguageChanged += SetupLanguageStatus;
        SettingsController.SoundChanged += SetupSoundState;
    }

    private void OnDisable()
    {
        SettingsController.LanguageChanged -= SetupLanguageStatus;
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
        _backButton.clicked += OnSettingsBackClicked;
        _privacyButton.clicked += OpenPrivacyPolicySite;
        _soundButton.clicked += () => SoundButtonClicked?.Invoke();
        _referralButton.clicked += () => _mainMenuUIManager.ShowReferralScreen();
    }
    
    private void SetupSoundState(SoundState soundState)
    {
        _soundButton.style.backgroundImage = new StyleBackground(soundState == SoundState.On ? SoundOn : SoundOff);
    }

    private void OpenPrivacyPolicySite()
    {
        Application.OpenURL("https://sites.google.com/view/capys-quest/privacy-policy?authuser=0");
    }

    private void OnSettingsBackClicked()
    {
        if (ScreenBefore is HomeScreen || ScreenBefore == null)
            _mainMenuUIManager.ShowHomeScreen();
        if (ScreenBefore is GameOverScreen)
            StartCoroutine(_mainMenuUIManager.ShowGameOverAfter(0f));
    }

    private void SetupScreenInfo()
    {
        _languageLabel.text = LocalizationManager.Localize("game_language");
        _soundLabel.text = LocalizationManager.Localize("sound_label");
    }

    private void SetupLanguageStatus()
    {
        LocalizationManager.Read();

        var language = LocalizationManager.Language;

        SetupScreenInfo();
        
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
