using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialScreen: MenuScreen
{
    public static event Action OnTutorialAccepted;
    private enum TutorialStep
    {
        First, Second
    }

    [SerializeField] private Sprite OkButton;


    private Button _tutorialNextButton;
    
    private VisualElement _firstTutorial;
    private VisualElement _secondTutorial;
    private Label _gameItemsLabel;

    private static string _tutorialNextButtonName = "NextButton";
    private static string _firstTutorialName = "FirstStep";
    private static string _secondTutorialName = "SecondStep";
    private static string _gameItemsLabelName = "GameItemLabel";

    private TutorialStep step = TutorialStep.First;

    [SerializeField] private DeviceType _deviceType;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _tutorialNextButton = _root.Q<Button>(_tutorialNextButtonName);
        _firstTutorial = _root.Q<VisualElement>(_firstTutorialName);
        _secondTutorial = _root.Q<VisualElement>(_secondTutorialName);
        _gameItemsLabel = _root.Q<Label>(_gameItemsLabelName);
    }
    
    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _tutorialNextButton.clicked += () => OnNextButtonClick();
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
            _gameItemsLabel.style.fontSize = new StyleLength(60);
        }
        else 
        {
            _gameItemsLabel.style.fontSize = new StyleLength(45);
        }
    }

    private void OnNextButtonClick()
    {
        if (step == TutorialStep.First) 
        {
            ButtonEvent.OnOpenMenuCalled();
            _firstTutorial.style.display = DisplayStyle.None;
            _secondTutorial.style.display = DisplayStyle.Flex;
            _tutorialNextButton.style.backgroundImage = new StyleBackground(OkButton);
            step = TutorialStep.Second;
        }
        else if (step == TutorialStep.Second)
        {
            ButtonEvent.OnEnterButtonCalled();
            PlayerPrefs.SetInt("isTutorialAccepted", 1);
            OnTutorialAccepted?.Invoke();
        }
    }
}
