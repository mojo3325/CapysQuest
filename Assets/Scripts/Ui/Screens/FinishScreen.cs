using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FinishScreen : MenuScreen
{
    public static event Action IsShown;
    public static event Action<Level> NextLevelClicked;

    private Button _okButton;
    private Button _homeButton;
    private Button _nextLevelButton;
    private Label _finishLabel;

    private VisualElement root;

    private static string _okButtonName = "OkButton";
    private static string _finishLabelName = "FinishText";
    private DeviceType _deviceType;

    private Level _finishedLevel;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        root = _root.Query<VisualElement>("FinishScreen");
        _okButton = root.Q<Button>(_okButtonName);
        _homeButton = root.Q<Button>("HomeButton");
        _nextLevelButton = root.Q<Button>("NextLevelButton");
        _finishLabel = root.Q<Label>(_finishLabelName);
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _okButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            _mainMenuUIManager.ShowHomeScreen();
        };
        _nextLevelButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            NextLevelClicked?.Invoke(_finishedLevel);
        };
        _homeButton.clicked += () =>
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

    private void SetupFinishScreen(Level level)
    {
        _finishedLevel = level;
        if(level < Level.LEVEL10)
        {
            _homeButton.style.display = DisplayStyle.Flex;
            _nextLevelButton.style.display = DisplayStyle.Flex;
            _okButton.style.display = DisplayStyle.None;
        }
        else
        {
            _homeButton.style.display = DisplayStyle.None;
            _nextLevelButton.style.display = DisplayStyle.None;
            _okButton.style.display = DisplayStyle.Flex;
        }
    }

    private void OnEnable()
    {
        FinishScreenController.IsReady += SetupFinishLabel;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
        CapyCharacter.OnFinishAchieved += SetupFinishScreen;
    }

    private void OnDisable()
    {
        FinishScreenController.IsReady -= SetupFinishLabel;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
        CapyCharacter.OnFinishAchieved += SetupFinishScreen;
    }
}
