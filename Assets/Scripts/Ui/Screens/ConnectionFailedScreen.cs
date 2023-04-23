using System;
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
        SetupSizes();
    }
    
    private void SetupSizes()
    {
        var devicetype = Tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _connectionText.style.fontSize = new StyleLength(60);
            _connectionButton.style.fontSize = new StyleLength(50);
        }
        else
        {
            _connectionText.style.fontSize = new StyleLength(45);
            _connectionButton.style.fontSize = new StyleLength(40);
        }
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
}
