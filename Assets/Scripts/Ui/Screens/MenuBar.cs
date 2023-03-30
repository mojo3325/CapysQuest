using System;
using Assets.SimpleLocalization;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuBar : MenuScreen
{
    public static event Action PlayButtonClicked;
    public static event Action SoundButtonClicked;

    [SerializeField]
    private Sprite SoundOn;
    [SerializeField]
    private Sprite SoundOff;

    private Button _playButton;
    private Button _soundButton;
    private Button _settingsButton;
    private Button _shopButton;
    private Button _referralButton;

    private static string _playButtonName = "PlayButton";
    private static string _soundButtonName = "SoundButton";
    private static string _settingsButtonName = "SettingsButton";
    private static string _shopButtonName = "ShopButton";
    private static string _referralButtonName = "ReferralButton";


    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _playButton = _root.Q<Button>(_playButtonName);
        _soundButton = _root.Q<Button>(_soundButtonName);
        _settingsButton = _root.Q<Button>(_settingsButtonName);
        _shopButton = _root.Q<Button>(_shopButtonName);
        _referralButton = _root.Q<Button>(_referralButtonName);
        
        _playButton.style.color = Color.white;
    }


    private void SetupScreen()
    {
        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
        }
        
        _playButton.text = LocalizationManager.Localize("play_button");

    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _playButton.clicked += () => OnPlayClick();
        _soundButton.clicked += () => SoundButtonClicked?.Invoke();
        _settingsButton.clicked += () => OnSettingsClick();
        _shopButton.clicked += () => _mainMenuUIManager.ShowShopScreen();
        _referralButton.clicked += () => _mainMenuUIManager.ShowReferralScreen();
    }

    private void SetupSoundState(SoundState soundState)
    {
        _soundButton.style.backgroundImage = new StyleBackground(soundState == SoundState.On ? SoundOn : SoundOff);
    }

    private void OnSettingsClick()
    {
        _mainMenuUIManager.ShowSettingsScreen();
    }

    private void OnPlayClick()
    {
        var isTutorialAccepted = PlayerPrefs.GetInt("isTutorialAccepted", 0);

        if (isTutorialAccepted == 1)
        {
            _mainMenuUIManager.ShowGameScreen();
            PlayButtonClicked?.Invoke();
        }
        else if (isTutorialAccepted == 0)
        {
            _mainMenuUIManager.ShowTutorialScreen();
        }
    }

    private void OnEnable()
    {
        ShowScreen();
        MenuBarController.SoundChanged += SetupSoundState;
        TutorialScreen.OnTutorialAccepted += OnPlayClick;
        SettingsController.LanguageChanged += SetupScreen;
        
        LocalizationManager.Read();
        SetupScreen();
    }
    
    private void OnDisable()
    {
        TutorialScreen.OnTutorialAccepted -= OnPlayClick;
        MenuBarController.SoundChanged -= SetupSoundState;
        SettingsController.LanguageChanged -= SetupScreen;
    }
}
