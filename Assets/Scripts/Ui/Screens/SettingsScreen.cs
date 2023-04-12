using Assets.SimpleLocalization;
using System;
using GoogleMobileAds.Api;
using Unity.RemoteConfig;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UIElements;
using ConfigResponse = Unity.Services.RemoteConfig.ConfigResponse;

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
    private Button _soundButton;
    private Button _referralButton;
    private Button _privacyButton;
    private Button _telegramButton;
    private Button _tiktokButton;
    private Button _instagramButton;
    
    private static string _languageLabelName = "LanguageText";
    private static string _languageButtonName = "LanguageButton";
    private static string _backButtonName = "BackButton";
    private static string _soundButtonName = "SoundButton";
    private static string _soundLabelName = "SoundText";
    private static string _referralButtonName = "ReferralButton";

    private static string _privacyButtonName = "PrivacyPolicyButton";
    private static string _tikTokButtonName = "TikTokButton";
    private static string _telegramButtonName = "TelegramButton";
    private static string _instagramButtonName = "InstagramButton";

    [SerializeField] private Sprite russianLang;
    [SerializeField] private Sprite englishLang;
    [SerializeField] private Sprite ukrainianLang;

    private string _gameInstagramLink = "";
    private string _gameTelegramLink = "";
    private string _gameTikTokLink = "";
    private string _privacyPolicyLink = "";

    private Tools _tools = new();
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _languageLabel = _root.Q<Label>(_languageLabelName);
        _languageButton = _root.Q<Button>(_languageButtonName);
        _backButton = _root.Q<Button>(_backButtonName);
        _soundButton = _root.Q<Button>(_soundButtonName);
        _soundLabel = _root.Q<Label>(_soundLabelName);
        _referralButton = _root.Q<Button>(_referralButtonName);
        
        _privacyButton = _root.Q<Button>(_privacyButtonName);
        _instagramButton = _root.Q<Button>(_instagramButtonName);
        _telegramButton = _root.Q<Button>(_telegramButtonName);
        _tiktokButton = _root.Q<Button>(_tikTokButtonName);
        
        SetupSizes();
    }

    private void OnEnable()
    {
        SettingsController.LanguageChanged += SetupLanguageStatus;
        SettingsController.SoundChanged += SetupSoundState;
        MenuManagerController.RemoteConfigInitialized += SetupConfigInfo;
    }

    private void OnDisable()
    {
        SettingsController.LanguageChanged -= SetupLanguageStatus;
        SettingsController.LanguageChanged -= SetupLanguageStatus;
        MenuManagerController.RemoteConfigInitialized -= SetupConfigInfo;
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
        SetupLanguageStatus();
    }

    private void SetupConfigInfo(ConfigResponse configResponse)
    {
        _gameInstagramLink = RemoteConfigService.Instance.appConfig.GetString("InstagramLink");
        _gameTikTokLink = RemoteConfigService.Instance.appConfig.GetString("TikTokLink");
        _gameTelegramLink = RemoteConfigService.Instance.appConfig.GetString("TelegramLink");
        _privacyPolicyLink = RemoteConfigService.Instance.appConfig.GetString("PrivacyPolicyLink");
    }
    
    private void SetupSizes()
    {
        var devicetype = _tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _languageLabel.style.fontSize = new StyleLength(50);
            _soundLabel.style.fontSize = new StyleLength(50);
        }
        else
        {
            _languageLabel.style.fontSize = new StyleLength(35);
            _soundLabel.style.fontSize = new StyleLength(35);
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _languageButton.clicked += () => LanguageButtonClicked?.Invoke();
        _backButton.clicked += _mainMenuUIManager.HideSettingsScreen;
        _soundButton.clicked += () => SoundButtonClicked?.Invoke();
        _referralButton.clicked += () => _mainMenuUIManager.ShowReferralScreen();
        _privacyButton.clicked += OpenPrivacyPolicySite;
        _instagramButton.clicked += OpenInstagramLink;
        _tiktokButton.clicked += OpenTikTokLink;
        _telegramButton.clicked += OpenTelegramLink;
    }
    
    private void SetupSoundState(SoundState soundState)
    {
        _soundButton.style.backgroundImage = new StyleBackground(soundState == SoundState.On ? SoundOn : SoundOff);
    }

    private void OpenTikTokLink()
    {
        var link = (_gameTikTokLink == "") ? "https://www.tiktok.com/@piderstudio" : _gameTikTokLink;
        Application.OpenURL(link);
    }
    
    private void OpenTelegramLink()
    {
        var link = (_gameTelegramLink == "") ? "https://t.me/pider_studio" : _gameTelegramLink;
        Application.OpenURL(link);
    }
    
    private void OpenInstagramLink()
    {
        var link = (_gameInstagramLink == "") ? "https://instagram.com/pider.studio?igshid=YmMyMTA2M2Y=" : _gameInstagramLink;
        Application.OpenURL(link);
    }

    private void OpenPrivacyPolicySite()
    {
        var link = (_privacyPolicyLink == "") ? "https://sites.google.com/view/capys-quest/privacy-policy?authuser=0" : _privacyPolicyLink;
        Application.OpenURL(link);
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
