using Assets.SimpleLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameScreen : MenuScreen
{
    [Header("GameScreenController")] [SerializeField]
    private GameScreenController _gameScreenController;
    
    public static event Action RightButtonClicked;
    public static event Action LeftButtonClicked;
    public static event Action IsShown;
    
    private Button _rightTapButton;
    private Button _leftTapButton;

    private Label _gameLabel;

    private VisualElement _progressRoot;
    private VisualElement _progressBar;
    private VisualElement _progressIndicator;

    private static string _rightTapButtonName = "RightTapButton";
    private static string _leftTapButtonName = "LeftTapButton";
    private static string _gameLabelName = "GameText";
    private static string _progressRootName = "ProgressRoot";
    private static string _progressBarName = "bar";
    private static string _progressIndicatorName = "indicator-image";
    
    private Tools _tools = new();

    private Coroutine _progressDisplayCoroutine;
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        
        _rightTapButton = _root.Q<Button>(_rightTapButtonName);
        _leftTapButton = _root.Q<Button>(_leftTapButtonName);
        _gameLabel = _root.Q<Label>(_gameLabelName);

        _progressRoot = _root.Q<VisualElement>(_progressRootName);
        _progressBar = _root.Q<VisualElement>(_progressBarName);
        _progressIndicator = _root.Q<VisualElement>(_progressIndicatorName);
        
        LocalizationManager.Read();
        SetupSizes();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
    }

    private void OnEnable()
    {
        ZoneController.OnZoneAchieved += ShowZoneReachedText;
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnCapyDied += (d, v) => OnCapyDie();
    }

    private void OnDisable()
    {
        ZoneController.OnZoneAchieved -= ShowZoneReachedText;
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnCapyDied += (d, v) => OnCapyDie();
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
        var devicetype = _tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _gameLabel.style.fontSize = new StyleLength(90);
        }
        else
        {
            _gameLabel.style.fontSize = new StyleLength(70);
        }
    }
    
    private void UpdateProgress(float currentProgress)
    {
        var width = (float)currentProgress / 1f * _progressRoot.contentRect.width;
        _progressBar.style.width = width;

        // Update progress indicator position
        var position = (float)currentProgress / 1f * _progressRoot.contentRect.width;
        _progressIndicator.style.left = position - _progressIndicator.contentRect.width / 2;
    }


    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _rightTapButton.clicked += () => RightButtonClicked?.Invoke();
        _leftTapButton.clicked += () => LeftButtonClicked?.Invoke();
    }

    private void ShowZoneReachedText(ZoneType zoneType)
    {
        var text = LocalizationManager.Localize(zoneType.ToString());
        StartCoroutine(ShowText(text));
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
