using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopScreen : MenuScreen
{
    private Label _noAdsLabel;
    private Button _backButton;

    private static string _backButtonName = "ShopBackButton";
    private static string _noAdsLabelName = "NoAdsLabel";
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        _backButton = _root.Q<Button>(_backButtonName);
        _noAdsLabel = _root.Q<Label>(_noAdsLabelName);
    }
    
    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += OnBackButtonClicked;
    }

    private void OnEnable()
    {
        SetupScreenInfo();
        
        SettingsController.LanguageChanged += SetupScreenInfo;
    }

    private void OnDisable()
    {
        SettingsController.LanguageChanged -= SetupScreenInfo;
    }

    private void SetupScreenInfo()
    {
        LocalizationManager.Read();
        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
        }

        _noAdsLabel.text = LocalizationManager.Localize("disableAd_label");
    }
    
    
    private void OnBackButtonClicked()
    {
        if (ScreenBefore is HomeScreen || ScreenBefore == null)
            _mainMenuUIManager.ShowHomeScreen();
        if (ScreenBefore is GameOverScreen)
            StartCoroutine(_mainMenuUIManager.ShowGameOverAfter(0f));
    }
}
