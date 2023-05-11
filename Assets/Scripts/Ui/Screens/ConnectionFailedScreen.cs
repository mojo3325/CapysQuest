using System;
using UnityEngine.UIElements;

public class ConnectionFailedScreen: MenuScreen
{
    public static event Action ConnectionButtonClicked;

    private Button _connectionButton;
    private Label _connectionText;

    private static string _connectionButtonName = "ConnectionFailedButton";
    private static string _connectionTextName = "ConnectionFailedLabel";
    private DeviceType _deviceType;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _connectionButton = _root.Q<Button>(_connectionButtonName);
        _connectionText = _root.Q<Label>(_connectionTextName);
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
        ConnectionScreenController.ConnectionIsChecked += CheckConnection;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
        ConnectionController.ConnectionIsChecked += OnConnectionChecked;
    }

    private void OnDisable()
    {
        ConnectionScreenController.ConnectionIsChecked -= CheckConnection;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
        ConnectionController.ConnectionIsChecked -= OnConnectionChecked;
    }
    
    private void OnConnectionChecked(bool isConnected)
    {
        if (!isConnected)
        {
            _mainMenuUIManager.ShowConnectionFailedScreen();
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        
        _connectionButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            ConnectionButtonClicked?.Invoke();
        };
    }

    private void CheckConnection(bool isConnected)
    {
        if (isConnected)
        {
            _mainMenuUIManager.HideConnectionFailedScreen();
        }
    }
}
