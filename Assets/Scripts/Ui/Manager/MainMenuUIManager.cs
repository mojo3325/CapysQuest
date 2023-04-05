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
    [SerializeField] LoadingScreen _loadingScreen;
    
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
        ShopController.PurchaseCalled += (v) => ShowLoadingScreen();
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
        ShopController.PurchaseCalled -= (v) => ShowLoadingScreen();
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

        if (_shopScreen != null)
            _allScreens.Add(_shopScreen);

        if (_referralScreen != null)
            _allScreens.Add(_referralScreen);

        if (_connectionFailedScreen != null)
            _allScreens.Add(_connectionFailedScreen);

        if (_versionFailedScreen != null)
            _allScreens.Add(_versionFailedScreen);

        if (_loadingScreen != null)
            _allScreens.Add(_loadingScreen);
}

    public void GoFromScreenToScreen(MenuScreen from = null, MenuScreen to = null)
    {
        foreach (var screen in _allScreens)
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

    private void HideAllScreens()
    {
        foreach(MenuScreen m in _allScreens)
        {
            m.HideScreen();
        }
    }

    private MenuScreen activeScreen()
    {
        foreach (var screen in _allScreens)
        {
            if (screen.IsVisible())
                return screen;
        }
        return null;
    }
    
    public void ShowIntAdScreen()
    {
        HideAllScreens();
    }

    public void ShowSettingsScreen()
    {
        GoFromScreenToScreen(from: activeScreen(), to: _settingsScreen);
    }
    
    public void HideSettingsScreen()
    {
        GoFromScreenToScreen(to: _settingsScreen.ScreenBefore);
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
        GoFromScreenToScreen(from: activeScreen(), to: _shopScreen);
    }
    
    public void HideShopScreen()
    {
        GoFromScreenToScreen(to: _shopScreen.ScreenBefore);
    }

    public void ShowReferralScreen()
    {
        GoFromScreenToScreen(from: _settingsScreen, to: _referralScreen);
    }

    public void HideReferralScreen()
    {
        GoFromScreenToScreen(to: _referralScreen.ScreenBefore);
    }

    private void ShowConnectionFailedScreen()
    {
        GoFromScreenToScreen(from: activeScreen(), to: _connectionFailedScreen);
    }
    
    public void HideConnectionFailedScreen()
    {
        if(_connectionFailedScreen.ScreenBefore == null || _connectionFailedScreen.ScreenBefore is ConnectionFailedScreen)
            ShowHomeScreen();
        else
            GoFromScreenToScreen(to: _connectionFailedScreen.ScreenBefore);
    }

    public void ShowVersionFailedScreen()
    {
        GoFromScreenToScreen(to: _versionFailedScreen);
    }

    private void ShowLoadingScreen()
    {
        GoFromScreenToScreen(from: activeScreen() ,to: _loadingScreen);
    }
    
    public void HideLoadingScreen()
    {
        GoFromScreenToScreen(to: _loadingScreen.ScreenBefore);
    }

    private void ShowFinishScreen()
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

    private void CheckConnection(bool isConnected)
    {
        if (!isConnected)
        {
            ShowConnectionFailedScreen();
        }
    }

    private void CheckVersion(VersionFetch fetch)
    {
        if (fetch == VersionFetch.Old)
        {
            ShowVersionFailedScreen();
        }
    }
}
