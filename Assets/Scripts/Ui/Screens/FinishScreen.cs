using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FinishScreen : MenuScreen
{
    public static event Action IsShown;

    private Button _okButton;
    private Label _finishLabel;

    private static string _okButtonName = "OkButton";
    private static string _finishLabelName = "FinishText";
    private DeviceType _deviceType;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        _okButton = _root.Q<Button>(_okButtonName);
        _finishLabel = _root.Q<Label>(_finishLabelName);
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _okButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            _mainMenuUIManager.ShowHomeScreen();
        };
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
    }

    private void SetupFinishLabel(string text)
    {
        _finishLabel.text = text;
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
            _finishLabel.style.fontSize = new StyleLength(45);
        }
        else
        {
            _finishLabel.style.fontSize = new StyleLength(35);
        }
    }

    private void OnEnable()
    {
        FinishScreenController.IsReady += SetupFinishLabel;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void OnDisable()
    {
        FinishScreenController.IsReady -= SetupFinishLabel;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
    }
}
