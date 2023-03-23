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

    [Header("Toolbars")]
    [SerializeField] MenuBar _menuBar;

    List<MenuScreen> _allScreens = new List<MenuScreen>();

    UIDocument _mainMenuDocument;
    public UIDocument MainMenuDocument => _mainMenuDocument;

    void OnEnable()
    {
        _mainMenuDocument = GetComponent<UIDocument>();
        SetupMenuScreens();
        SubscribeOnEvents();
        IsEnabled?.Invoke();
    }

    private void SubscribeOnEvents()
    {
        CapyCharacter.OnCapyDied += ShowGameOverAfterDie;
        MenuManagerController.ConnectionIsChecked += CheckConnection;
        MenuManagerController.VersionIsChecked += CheckVersion;
        CapyCharacter.OnFinishAchieved += ShowFinishScreen;
    }

    private void OnDisable()
    {
        CapyCharacter.OnCapyDied -= ShowGameOverAfterDie;
        MenuManagerController.ConnectionIsChecked -= CheckConnection;
        MenuManagerController.VersionIsChecked -= CheckVersion;
        CapyCharacter.OnFinishAchieved -= ShowFinishScreen;
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
    }

    void ShowMenuScreen(MenuScreen menuScreen)
    {
        foreach (MenuScreen m in _allScreens)
        {
            if (m == menuScreen)
            {
                m?.ShowScreen();
            }
            else
            {
                m?.HideScreen();
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

    public void ShowSettingsScreen()
    {
        if (_homeScreen.IsVisible())
            _settingsScreen.ScreenBefore = ScreenBefore.HomeScreen;
        if (_gameOverScreen.IsVisible() == true)
            _settingsScreen.ScreenBefore = ScreenBefore.GameOver;

        if (_menuBar.IsVisible() == true)
            HideMenuBar();

        ShowMenuScreen(_settingsScreen);
    }

    public IEnumerator ShowGameOverAfter(float delay = 1.5f)
    {
        yield return new WaitForSeconds(delay);
        ShowMenuScreen(_gameOverScreen);

        if (_menuBar.IsVisible() == false)
            ShowMenuBar();
    }

    public void ShowHomeScreen()
    {
        ShowMenuScreen(_homeScreen);

        if(_menuBar.IsVisible() == false)
            ShowMenuBar();
    }

    private void ShowMenuBar()
    {
        _menuBar.ShowScreen();
    }

    private void HideMenuBar()
    {
        _menuBar.HideScreen();
    }

    public void ShowConnectionFailedScreen()
    {
        HideAllScreens();
        _connectionFailedScreen.ShowScreen();

        if (_menuBar.IsVisible() == true)
             HideMenuBar();
    }

    public void ShowVersionFailedScreen()
    {
        HideAllScreens();
        _versionFailedScreen.ShowScreen();

        if (_menuBar.IsVisible() == true)
            HideMenuBar();
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
        ShowMenuScreen(_finishScreen);
        if (_menuBar.IsVisible() == true)
            HideMenuBar();
    }

    public void ShowTutorialScreen()
    {
        ShowMenuScreen(_tutorialScreen);
        if (_menuBar.IsVisible() == true)
             HideMenuBar();
    }

    public void ShowGameScreen()
    {
        ShowMenuScreen(_gameScreen);
        if (_menuBar.IsVisible() == true)
            HideMenuBar();
    }

    private void ShowGameOverAfterDie(DieType dieType, Vector3 vector3)
    {
        if(_connectionFailedScreen.IsVisible() == false && _versionFailedScreen.IsVisible() == false)
            StartCoroutine(ShowGameOverAfter());
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
                _connectionFailedScreen.ScreenBefore = ScreenBefore.HomeScreen;
                
            if (_gameOverScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = ScreenBefore.GameOver;

            if (_settingsScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = ScreenBefore.SettingsScreen;

            if (_tutorialScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = ScreenBefore.TutorialScreen;

            if (_finishScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = ScreenBefore.FinishScreen;

            if (_gameScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = ScreenBefore.GameOver;

            if (_versionFailedScreen.IsVisible())
                _connectionFailedScreen.ScreenBefore = ScreenBefore.VersionFailedScreen;

            ShowConnectionFailedScreen();
        }
    }

    public void CheckVersion(VersionFetch fetch)
    {
        if (fetch == VersionFetch.Old)
        {
            ShowVersionFailedScreen();
        }
    }
}
