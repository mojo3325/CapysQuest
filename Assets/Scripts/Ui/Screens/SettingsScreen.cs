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
    public static event Action SoundButtonClicked;

    [SerializeField]
    private Sprite SoundOn;
    [SerializeField]
    private Sprite SoundOff;

    private Label _soundLabel;
    private Label _socialLabel;
    private Label _otherLabel;
    private Label _topBarLabel;
    
    private Button _backButton;
    private Button _soundButton;
    private Button _privacyButton;
    private Button _telegramButton;
    private Button _tiktokButton;
    private Button _instagramButton;
    private Button _codeInputButton;
    
    private static string _backButtonName = "BackButton";
    private static string _soundButtonName = "SoundButton";
    private static string _soundLabelName = "SoundText";
    private static string _socialLabelName = "SocialText";
    private static string _otherLabelName = "OtherText";
    private static string _topBarLabelName = "settings_top_bar_label";
    
    private static string _privacyButtonName = "PrivacyPolicyButton";
    private static string _tikTokButtonName = "TikTokButton";
    private static string _telegramButtonName = "TelegramButton";
    private static string _instagramButtonName = "InstagramButton";
    private static string _codeInputButtonName = "CodeInputButton";

    private string _gameInstagramLink = "";
    private string _gameTelegramLink = "";
    private string _gameTikTokLink = "";
    private string _privacyPolicyLink = "";
    [SerializeField] private DeviceType _deviceType;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _backButton = _root.Q<Button>(_backButtonName);
        _soundButton = _root.Q<Button>(_soundButtonName);
        _soundLabel = _root.Q<Label>(_soundLabelName);
        _socialLabel = _root.Q<Label>(_socialLabelName);
        _otherLabel = _root.Q<Label>(_otherLabelName);
        _topBarLabel = _root.Q<Label>(_topBarLabelName);
        _privacyButton = _root.Q<Button>(_privacyButtonName);
        _instagramButton = _root.Q<Button>(_instagramButtonName);
        _telegramButton = _root.Q<Button>(_telegramButtonName);
        _tiktokButton = _root.Q<Button>(_tikTokButtonName);
        _codeInputButton = _root.Q<Button>(_codeInputButtonName);
    }

    private void OnEnable()
    {
        SettingsController.SoundChanged += SetupSoundState;
        ConfigController.RemoteConfigInitialized += SetupConfigInfo;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void OnDisable()
    {
        ConfigController.RemoteConfigInitialized -= SetupConfigInfo;
        ConfigController.RemoteConfigInitialized -= SetupConfigInfo;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
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
        _deviceType = _mainMenuUIManager.DeviceController.DeviceType;

        if (string.IsNullOrEmpty(_deviceType.ToString()))
        {
            _mainMenuUIManager.DeviceController.SyncDeviceType();
            _deviceType = _mainMenuUIManager.DeviceController.DeviceType;
        }
        
        if (_deviceType == DeviceType.Phone)
        {
            _soundLabel.style.fontSize = new StyleLength(45);
            _socialLabel.style.fontSize = new StyleLength(45);
            _otherLabel.style.fontSize = new StyleLength(45);
            _topBarLabel.style.fontSize = new StyleLength(60);
        }
        
        if(_deviceType == DeviceType.Tablet)
        {
            _soundLabel.style.fontSize = new StyleLength(35);
            _socialLabel.style.fontSize = new StyleLength(35);
            _otherLabel.style.fontSize = new StyleLength(35);
            _topBarLabel.style.fontSize = new StyleLength(40);
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideSettingsScreen();
        };
        _soundButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            SoundButtonClicked?.Invoke();
        };
        _privacyButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            OpenPrivacyPolicySite();
        };
        _instagramButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            OpenInstagramLink();
        };
        _tiktokButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            OpenTikTokLink();
        };
        _telegramButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            OpenTelegramLink();
        };
        _codeInputButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            _mainMenuUIManager.ShowCodeInputScreen();
        };
    }
    
    private void SetupSoundState(SoundState soundState)
    {
        _soundButton.style.backgroundImage = new StyleBackground(soundState == SoundState.On ? SoundOn : SoundOff);
    }

    private void OpenTikTokLink()
    {
        Application.OpenURL(_gameTikTokLink);
    }
    
    private void OpenTelegramLink()
    {
        Application.OpenURL(_gameTelegramLink);
    }
    
    private void OpenInstagramLink()
    {
        Application.OpenURL(_gameInstagramLink);
    }

    private void OpenPrivacyPolicySite()
    {
        Application.OpenURL(_privacyPolicyLink);
    }
}
