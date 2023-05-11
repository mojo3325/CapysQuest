using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomizationScreen : MenuScreen
{
    [Header("Capy")]
    [SerializeField] private GameObject _animationCapy;

    public static event Action BackButtonClicked;
    public static event Action NextButtonClicked;
    public static event Action PreviousButtonClicked;
    
    private Button _backButton;
    private Button _nextSkinButton;
    private Button _previousSkinButton;
    
    private VisualElement root;
    
    private VisualElement _previousSkin;
    private VisualElement _nextSkin;
    
    private Label _topBarLabel;
    
    private static string _topBarLabelName = "top_bar_label";

    private static string _backButtonName = "back_button";
    
    [SerializeField] private DeviceType _deviceType;

    public VisualElement PreviousSkin
    {
        get => _previousSkin;
        set => _previousSkin = value;
    }

    public VisualElement NextSkin
    {
        get => _nextSkin;
        set => _nextSkin = value;
    }

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        root = _root.Query<VisualElement>("CustomizationScreen");
        
        _backButton = root.Q<Button>(_backButtonName);
        _nextSkinButton = root.Q<Button>("NextButton");
        _previousSkinButton = root.Q<Button>("PreviousButton");

        _previousSkin = root.Q<VisualElement>("PreviousSkin");
        _nextSkin = root.Q<VisualElement>("NextSkin");
        
        _topBarLabel = root.Q<Label>(_topBarLabelName);
    }

    private void OnEnable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void OnDisable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
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
            _topBarLabel.style.fontSize = new StyleLength(60);
            _animationCapy.transform.localScale = new Vector3(90.17f, 90.17f, 90.17f);
        }
        if(_deviceType == DeviceType.Tablet)
        {
            _topBarLabel.style.fontSize = new StyleLength(40);
            _animationCapy.transform.localScale = new Vector3(40, 40, 40);
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += () =>
        {
            BackButtonClicked?.Invoke();
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideCustomizationScreen();
        };

        _nextSkinButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            NextButtonClicked?.Invoke();
        };

        _previousSkinButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            PreviousButtonClicked?.Invoke();
        };
    }
}
