using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopScreen : MenuScreen
{
    private Label _emptyLabel;
    private Button _backButton;

    private static string _backButtonName = "ShopBackButton";
    private static string _emptyLabelName = "EmptyLabel";
    
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        _backButton = _root.Q<Button>(_backButtonName);

    }
    
    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += OnBackButtonClicked;
    }

    private void OnBackButtonClicked()
    {
        if (ScreenBefore is HomeScreen || ScreenBefore == null)
            _mainMenuUIManager.ShowHomeScreen();
        if (ScreenBefore is GameOverScreen)
            StartCoroutine(_mainMenuUIManager.ShowGameOverAfter(0f));
    }
}
