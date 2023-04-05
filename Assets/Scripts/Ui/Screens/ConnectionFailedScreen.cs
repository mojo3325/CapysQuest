using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionFailedScreen: MenuScreen
{
    public static event Action ConnectionButtonClicked;

    private Button _connectionButton;
    private Label _connectionText;

    private static string _connectionButtonName = "ConnectionFailedButton";
    private static string _connectionTextName = "ConnectionFailedLabel";

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _connectionButton = _root.Q<Button>(_connectionButtonName);
        _connectionText = _root.Q<Label>(_connectionTextName);
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        SetupConnectionAlert();
    }

    private void OnEnable()
    {
        ConnectionController.ConnectionIsChecked += CheckConnection;
    }

    private void OnDisable()
    {
        ConnectionController.ConnectionIsChecked -= CheckConnection;
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _connectionButton.clicked += () => ConnectionButtonClicked?.Invoke();
    }

    private void CheckConnection(bool isConnected)
    {
        if (isConnected)
        {
            _mainMenuUIManager.HideConnectionFailedScreen();
        }
    }

    private void SetupConnectionAlert()
    {
        LocalizationManager.Read();

        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
            _connectionButton.style.color = Color.white;
            _connectionText.text = LocalizationManager.Localize("connection_lost");
            _connectionButton.text = LocalizationManager.Localize("connection_try");
        }else
        {
            _connectionButton.style.color = Color.white;
            _connectionText.text = "There is no connection...";
            _connectionButton.text = "Reconnect";
        }

    }
}
