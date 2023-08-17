using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : MenuScreen
{

    [SerializeField] private GameScreenController gameScreenController;
    [SerializeField] private CapyController characterController;

    public static event Action IsShown;

    private Label _gameOverLabel;
    private VisualElement root;
    private VisualElement _progressRoot;
    private VisualElement _progressBar;
    private VisualElement _progressIndicator;

    private static string _progressRootName = "ProgressRoot";
    private static string _progressBarName = "bar";
    private static string _progressIndicatorName = "indicator-image";

    private static string _gameOverLabelName = "GameOverLabel";
      
    private DeviceType _deviceType;

    private bool hasShownDeathAnimation = false;
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = true;
        _gameOverLabel = _root.Q<Label>(_gameOverLabelName);
        root = _root.Query<VisualElement>("GameOverScreen");

        _progressRoot = root.Q<VisualElement>(_progressRootName);
        _progressBar = root.Q<VisualElement>(_progressBarName);
        _progressIndicator = root.Q<VisualElement>(_progressIndicatorName);
    }

    private void OnEnable()
    {        
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
        MenuBar.PlayButtonClicked += (level) => ResetProgressState();
    }

    private void OnDisable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
        MenuBar.PlayButtonClicked -= (level) => ResetProgressState();
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

        }
        if(_deviceType == DeviceType.Tablet)
        {
;
        }
    }

    private void ResetProgressState()
    {
        _progressBar.style.width = 0;
        _progressIndicator.style.left = -_progressIndicator.contentRect.width / 2;
        hasShownDeathAnimation = false;
    }


    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();

        if (!hasShownDeathAnimation)
        {
            SetupGameOverScreen();
            hasShownDeathAnimation = true;
        }
    }

    private void SetupGameOverScreen()
    {
        _progressIndicator.style.backgroundImage = new StyleBackground(characterController.selectedSkinIcon);
        StartCoroutine(AnimateProgressOnDeath(targetProgress: gameScreenController.CurrentLevelProgress));
    }

    private IEnumerator AnimateProgressOnDeath(float targetProgress, float duration = 2f)
    {
        float initialProgress = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            var progress = Mathf.Lerp(initialProgress, targetProgress, t);

            var width = progress * _progressRoot.contentRect.width;
            _progressBar.style.width = width;

            var position = progress * _progressRoot.contentRect.width;
            _progressIndicator.style.left = position - _progressIndicator.contentRect.width / 2;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _progressBar.style.width = targetProgress * _progressRoot.contentRect.width;
        _progressIndicator.style.left = targetProgress * _progressRoot.contentRect.width - _progressIndicator.contentRect.width / 2;
    }
}