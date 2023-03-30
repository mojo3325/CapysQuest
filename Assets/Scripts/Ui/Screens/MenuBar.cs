using System;
using Assets.SimpleLocalization;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuBar : MenuScreen
{
    public static event Action PlayButtonClicked;

    private Button _playButton;
    private Button _settingsButton;
    private Button _shopButton;

    private static string _playButtonName = "PlayButton";
    private static string _settingsButtonName = "SettingsButton";
    private static string _shopButtonName = "ShopButton";
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _playButton = _root.Q<Button>(_playButtonName);
        _settingsButton = _root.Q<Button>(_settingsButtonName);
        _shopButton = _root.Q<Button>(_shopButtonName);
        
        _playButton.style.color = Color.white;
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _playButton.clicked += () => OnPlayClick();
        _settingsButton.clicked += () => OnSettingsClick();
        _shopButton.clicked += () => _mainMenuUIManager.ShowShopScreen();
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
        TutorialScreen.OnTutorialAccepted += OnPlayClick;
        LocalizationManager.Read();
    }
    
    private void OnDisable()
    {
        TutorialScreen.OnTutorialAccepted -= OnPlayClick;
    }
}
