using System;
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

    private static string _playButtonName = "PlayButton";
    private static string _soundButtonName = "SoundButton";
    private static string _settingsButtonName = "SettingsButton";


    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _playButton = _root.Q<Button>(_playButtonName);
        _soundButton = _root.Q<Button>(_soundButtonName);
        _settingsButton = _root.Q<Button>(_settingsButtonName);
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _playButton.clicked += () => OnPlayClick();
        _soundButton.clicked += () => SoundButtonClicked?.Invoke();
        _settingsButton.clicked += () => OnSettingsClick();
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
    }

    private void OnDisable()
    {
        TutorialScreen.OnTutorialAccepted -= OnPlayClick;
        MenuBarController.SoundChanged -= SetupSoundState;
    }
}
