using Assets.SimpleLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class MainMenuUIManager : MonoBehaviour
{
    public static event Action IsEnabled;
    public static event Action IsPaused;
    public static event Action IsFocused;

    [Header("Menu Screens")]
    [SerializeField] HomeScreen _homeScreen;
    [SerializeField] SettingsScreen _settingsScreen;
    [SerializeField] GameOverScreen _gameOverScreen;
    [SerializeField] ConnectionFailedScreen _connectionFailedScreen;
    [SerializeField] VersionFailedScreen _versionFailedScreen;
    [SerializeField] FinishScreen _finishScreen;
    [SerializeField] TutorialScreen _tutorialScreen;
    [SerializeField] GameScreen _gameScreen;
    [SerializeField] ShopScreen _shopScreen;
    [SerializeField] ReferralScreen _referralScreen; 
    
    [Header("Toolbars")]
    [SerializeField] MenuBar _menuBar;

    List<MenuScreen> _allScreens = new List<MenuScreen>();

    UIDocument _mainMenuDocument;
    public UIDocument MainMenuDocument => _mainMenuDocument;

    private Coroutine _gameOverCoroutine;

    void OnEnable()
    {
        _mainMenuDocument = GetComponent<UIDocument>();
        SetupMenuScreens();
        SubscribeOnEvents();
        IsEnabled?.Invoke();
    }

    private void SubscribeOnEvents()
    {
        CapyCharacter.OnCapyDied += (d, v) => ShowGameOverAfterDie();
        CapyController.OnTimeLost += ShowGameOverAfterDie;
        MenuManagerController.ConnectionIsChecked += CheckConnection;
        MenuManagerController.VersionIsChecked += CheckVersion;
        CapyCharacter.OnFinishAchieved += ShowFinishScreen;
        CapyController.CapyDiedThreeTimes += ShowIntAdScreen;
        MenuBar.PlayButtonClicked += ResetScreensCoroutines;
    }

    private void OnDisable()
    {
        CapyCharacter.OnCapyDied -= (_, _) => ShowGameOverAfterDie();
        CapyController.OnTimeLost -= ShowGameOverAfterDie;
        MenuManagerController.ConnectionIsChecked -= CheckConnection;
        MenuManagerController.VersionIsChecked -= CheckVersion;
        CapyCharacter.OnFinishAchieved -= ShowFinishScreen;
        CapyController.CapyDiedThreeTimes -= ShowIntAdScreen;
        MenuBar.PlayButtonClicked -= ResetScreensCoroutines;
    }

    void SetupMenuScreens()
    {
        if (_homeScreen != null)
            _allScreens.Add(_homeScreen);

        if (_settingsScreen != null)
            _allScreens.Add(_settingsScreen);

        if (_gameOverScreen != null)
            _allScreens.Add(_gameOverScreen);

        if (_finishScreen != null)
            _allScreens.Add(_finishScreen);

        if (_tutorialScreen != null)
            _allScreens.Add(_tutorialScreen);

        if (_gameScreen != null)
            _allScreens.Add(_gameScreen);
        
        if(_shopScreen != null)
            _allScreens.Add(_shopScreen);
        
        if(_referralScreen != null)
            _allScreens.Add(_referralScreen);
    }

    public void GoFromScreenToScreen(MenuScreen from = null, MenuScreen to = null)
    {
        foreach (MenuScreen screen in _allScreens)
        {
            if (screen == to)
            {
                if (from != null && to != null)
                    to.ScreenBefore = from;

                screen?.ShowScreen();

                if (screen.ShowMenuBar)
                    ShowMenuBar();
                else
                    HideMenuBar();
            }
            else
            {
                screen?.HideScreen();
            }
        }
    }

    void HideAllScreens()
    {
        foreach(MenuScreen m in _allScreens)
        {
            m.HideScreen();
        }
    }

    public void ShowIntAdScreen()
    {
        HideAllScreens();
    }

    public void ShowSettingsScreen()
    {
        if (_homeScreen.IsVisible())
            GoFromScreenToScreen(from: _homeScreen, to: _settingsScreen);
        if (_gameOverScreen.IsVisible() == true)
            GoFromScreenToScreen(from: _gameOverScreen, to: _settingsScreen);
    }

    public IEnumerator ShowGameOverAfter(float delay = 1.5f)
    {
        yield return new WaitForSeconds(delay);
        GoFromScreenToScreen(to: _gameOverScreen);
    }

    public void ShowHomeScreen()
    {
        GoFromScreenToScreen(to : _homeScreen);
    }

    private void ShowMenuBar()
    {
        _menuBar.ShowScreen();
    }

    private void HideMenuBar()
    {
        _menuBar.HideScreen();
    }

    public void ShowShopScreen()
    {
        if (_homeScreen.IsVisible())
            GoFromScreenToScreen(from: _homeScreen, to: _shopScreen);
        if (_gameOverScreen.IsVisible() == true)
            GoFromScreenToScreen(from: _gameOverScreen, to: _shopScreen);
    }

    public void ShowReferralScreen()
    {
        if (_homeScreen.IsVisible())
            GoFromScreenToScreen(from: _homeScreen, to: _referralScreen);
        if (_gameOverScreen.IsVisible() == true)
            GoFromScreenToScreen(from: _gameOverScreen, to: _referralScreen);
    }

    public void ShowConnectionFailedScreen()
    {
        HideAllScreens();
        _connectionFailedScreen.ShowScreen();
    }

    public void ShowVersionFailedScreen()
    {
        HideAllScreens();
        _versionFailedScreen.ShowScreen();
    }

    public void HideVersionFailedScreen()
    {
        _versionFailedScreen.HideScreen();
    }

    public void HideConnectionFailedScreen()
    {
        _connectionFailedScreen.HideScreen();
    }

    public void ShowFinishScreen()
    {
        GoFromScreenToScreen(to: _finishScreen);
    }

    public void ShowTutorialScreen()
    {   
        GoFromScreenToScreen(to :_tutorialScreen);
    }

    public void ShowGameScreen()
    {
        GoFromScreenToScreen(to :_gameScreen);
    }

    private void ShowGameOverAfterDie()
    {
        if(_connectionFailedScreen.IsVisible() == false && _versionFailedScreen.IsVisible() == false && _finishScreen.IsVisible() == false)
            _gameOverCoroutine = StartCoroutine(ShowGameOverAfter());
    }

    private void ResetScreensCoroutines()
    {
        if(_gameOverCoroutine != null)
            StopCoroutine(_gameOverCoroutine);
    }


    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            IsPaused?.Invoke();
        }
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            IsFocused?.Invoke();
        }
    }

    public void CheckConnection(bool isConnected)
    {
        if (!isConnected)
        {
            if (_homeScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _homeScreen;
                
            if (_gameOverScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _gameOverScreen;

            if (_settingsScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _settingsScreen;

            if (_tutorialScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _tutorialScreen;

            if (_finishScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _finishScreen;

            if (_gameScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _gameOverScreen;

            if (_versionFailedScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = _versionFailedScreen;

            ShowConnectionFailedScreen();

            if (_menuBar.IsVisible())
                HideMenuBar();
        }
    }

    public void CheckVersion(VersionFetch fetch)
    {
        if (fetch == VersionFetch.Old)
        {
            ShowVersionFailedScreen();

            if (_menuBar.IsVisible())
                HideMenuBar();
        }
    }
}
