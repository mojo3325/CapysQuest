using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameScreen : MenuScreen
{
    [Header("GameScreenController")]
    [SerializeField] private GameScreenController _gameScreenController;
    public static event Action RightButtonClicked;
    public static event Action LeftButtonClicked;
    public static event Action IsShown;

    [Header("CharacterController")]
    [SerializeField] private CapyController characterController;

    private Button _rightTapButton;
    private Button _leftTapButton;

    private Label _gameLabel;

    private VisualElement _progressRoot;
    private VisualElement _progressBar;
    private VisualElement _progressIndicator;

    private VisualElement root;

    private static string _rightTapButtonName = "RightTapButton";
    private static string _leftTapButtonName = "LeftTapButton";
    private static string _gameLabelName = "GameText";
    private static string _progressRootName = "ProgressRoot";
    private static string _progressBarName = "bar";
    private static string _progressIndicatorName = "indicator-image";
    private Dictionary<string, object> _gameStrings;

    private Coroutine _progressDisplayCoroutine;
    private DeviceType _deviceType;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        root = _root.Query<VisualElement>("GameScreen");
        
        _rightTapButton = root.Q<Button>(_rightTapButtonName);
        _leftTapButton = root.Q<Button>(_leftTapButtonName);
        _gameLabel = root.Q<Label>(_gameLabelName);

        _progressRoot = root.Q<VisualElement>(_progressRootName);
        _progressBar = root.Q<VisualElement>(_progressBarName);
        _progressIndicator = root.Q<VisualElement>(_progressIndicatorName);
    }
    
    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
        _progressIndicator.style.backgroundImage = new StyleBackground(characterController.selectedSkinIcon);
    }

    private void OnEnable()
    {
        ZoneController.OnLevelAchieved += ShowZoneReachedText;
        MenuBar.PlayButtonClicked += (level) => OnPlayClick();
        CapyCharacter.OnCapyDied += (d, v) => OnCapyDie();
        CapyCharacter.JetpackClaimed += (f) => OnJetpackClaimed();
        CapyCharacter.HelmetClaimed += OnHelmetClaimed;
        GameStringsController.GameStringsInitialized += GameStringsInit;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void GameStringsInit(Dictionary<string, object> strings)
    {
        _gameStrings = strings;
    }
    

    private void OnJetpackClaimed()
    {
        StartCoroutine(ShowText(_gameStrings["jetpack_booster"].ToString()));
    }
    
    private void OnHelmetClaimed()
    {
        StartCoroutine(ShowText(_gameStrings["helm_booster"].ToString()));
    }

    private void OnDisable()
    {
        ZoneController.OnLevelAchieved -= ShowZoneReachedText;
        MenuBar.PlayButtonClicked -= (level) => OnPlayClick();
        CapyCharacter.OnCapyDied -= (d, v) => OnCapyDie();
        CapyCharacter.JetpackClaimed -= (f) => OnJetpackClaimed();
        CapyCharacter.HelmetClaimed -= OnHelmetClaimed;
        GameStringsController.GameStringsInitialized -= GameStringsInit;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
    }
    
    
    private void OnPlayClick()
    {
        if(_progressDisplayCoroutine != null)
            StopCoroutine(_progressDisplayCoroutine);

        _progressDisplayCoroutine = StartCoroutine(ProgressDisplay());
    }

    private void OnCapyDie()
    {
        if(_progressDisplayCoroutine != null)
            StopCoroutine(_progressDisplayCoroutine);
    }
    
    private IEnumerator ProgressDisplay()
    {
        while (true)
        {
            var currentProgress = _gameScreenController.CurrentLevelProgress;
        
            var width = (float)currentProgress / 1f * _progressRoot.contentRect.width;
            _progressBar.style.width = width;
        
            var position = (float)currentProgress / 1f * _progressRoot.contentRect.width;
            _progressIndicator.style.left = position - _progressIndicator.contentRect.width / 2;
            
            yield return null;
        }
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
            _gameLabel.style.fontSize = new StyleLength(90);
        }
        else
        {
            _gameLabel.style.fontSize = new StyleLength(70);
        }
    }
    
    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _rightTapButton.clicked += () => RightButtonClicked?.Invoke();
        _leftTapButton.clicked += () => LeftButtonClicked?.Invoke();
    }

    private void ShowZoneReachedText(Level level)
    {
        var text = _gameStrings[level.ToString()];
        StartCoroutine(ShowText(text.ToString()));
    }
    
    private IEnumerator ShowText(string text = "") 
    {
        _gameLabel.style.display = DisplayStyle.Flex;
        _gameLabel.style.opacity = 1f;
        _gameLabel.text = text;
        yield return new WaitForSeconds(1.5f);
        _gameLabel.style.display = DisplayStyle.None;
        _gameLabel.style.opacity = 0f;
    }
}
