using Assets.Scripts.Ui.Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Cache = UnityEngine.Cache;


[RequireComponent(typeof(UIDocument))]
public class MainMenuUIManager : MonoBehaviour
{
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
    [SerializeField] AccountScreen _accountScreen;
    [SerializeField] OperationStatusScreen _operationStatusScreen;
    [SerializeField] GameAlertScreen  _gameAlertScreen;
    [SerializeField] CodeInputScreen _codeInputScreen;
    [SerializeField] CustomizationScreen _customizationScreen;
    [SerializeField] SignOutScreen _signOutScreen;
    [SerializeField] LoadingScreen _loadingScreen;
    [SerializeField] LevelMenuScreen _levelMenuScreen;
    
    [Header("Toolbars")]
    [SerializeField] MenuBar _menuBar;
    
    [Header("Controllers")]
    [SerializeField] private DeviceController _deviceController;

    [Header("Cameras")] 
    [SerializeField] private GameObject _gameCamera;

    [SerializeField] private GameObject _menuCamera;

    List<MenuScreen> _allScreens = new List<MenuScreen>();

    UIDocument _mainMenuDocument;
    public UIDocument MainMenuDocument => _mainMenuDocument;
    public DeviceController DeviceController => _deviceController; 
    private Coroutine _gameOverCoroutine;

    private void OnEnable()
    {
        _mainMenuDocument = GetComponent<UIDocument>();
        SetupMenuScreens();
        SubscribeOnEvents();
    }

    private void SubscribeOnEvents()
    {
        CapyCharacter.OnCapyDied += (d, v) => ShowGameOverAfterDie();
        ConfigController.VersionIsChecked += CheckVersion;
        CapyCharacter.OnFinishAchieved += (level) => ShowFinishScreen();
        CapyController.CapyDiedThreeTimes += ShowIntAdScreen;
        MenuBar.PlayButtonClicked += (it) => ResetScreensCoroutines();
        ShopController.PurchaseCalled += (v) => ShowOperationStatusScreen();
        ConfigController.GameOnTechnicalBreak += () => ShowGameAlert("Игра временно на техническом перерыве)");
    }

    private void OnDisable()
    {
        CapyCharacter.OnCapyDied -= (_, _) => ShowGameOverAfterDie();
        ConfigController.VersionIsChecked -= CheckVersion;
        CapyCharacter.OnFinishAchieved -= (level) => ShowFinishScreen();
        CapyController.CapyDiedThreeTimes -= ShowIntAdScreen;
        MenuBar.PlayButtonClicked -= (it) =>  ResetScreensCoroutines();
        ShopController.PurchaseCalled -= (v) => ShowOperationStatusScreen();
        ConfigController.GameOnTechnicalBreak -= () => ShowGameAlert("Игра временно на техническом перерыве)");
    }

    private void SetupMenuScreens()
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

        if (_accountScreen != null)
            _allScreens.Add(_accountScreen);

        if (_connectionFailedScreen != null)
            _allScreens.Add(_connectionFailedScreen);

        if (_versionFailedScreen != null)
            _allScreens.Add(_versionFailedScreen);

        if (_operationStatusScreen != null)
            _allScreens.Add(_operationStatusScreen);
        
        if(_gameAlertScreen != null)
            _allScreens.Add(_gameAlertScreen);
        
        if(_codeInputScreen != null)
            _allScreens.Add(_codeInputScreen);
        
        if(_signOutScreen != null)
            _allScreens.Add(_signOutScreen);
        
        if(_customizationScreen != null)
            _allScreens.Add(_customizationScreen);

        if (_loadingScreen != null)
            _allScreens.Add(_loadingScreen);

        if (_levelMenuScreen != null)
            _allScreens.Add(_levelMenuScreen);
    }

    private void SyncDeviceType()
    {
        _deviceController.SyncDeviceType();
    }

    private void Awake() => SyncDeviceType();

    private void GoFromScreenToScreen(MenuScreen from = null, MenuScreen to = null)
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

    public MenuScreen ActiveScreen()
    {
        foreach (var screen in _allScreens)
        {
            if (screen.IsVisible())
                return screen;
        }
        return null;
    }

    private void SetGameCameraActive()
    {
        _menuCamera.SetActive(false);
        _gameCamera.SetActive(true);
    }

    private void SetMenuCameraActive()
    {
        _menuCamera.SetActive(true);
        _gameCamera.SetActive(false);
    }

    private void SetupHomeScreenBackground()
    {
        _gameCamera.transform.localPosition = new Vector3(-913.821777f, -298.818604f, -32);
    }
    
    private void ShowIntAdScreen()
    {
        HideAllScreens();
    }

    public void ShowSettingsScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(), to: _settingsScreen);
    }
    
    public void HideSettingsScreen()
    {
        GoFromScreenToScreen(to: _settingsScreen.ScreenBefore);
    }

    private IEnumerator ShowGameOverAfter(float delay = 1.5f)
    {
        yield return new WaitForSeconds(delay);
        SetGameCameraActive();
        GoFromScreenToScreen(to: _gameOverScreen);
    }

    public void ShowHomeScreen()
    {
        SetGameCameraActive();
        SetupHomeScreenBackground();
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
        GoFromScreenToScreen(from: ActiveScreen(), to: _shopScreen);
    }
    
    public void HideShopScreen()
    {
        GoFromScreenToScreen(to: _shopScreen.ScreenBefore);
    }

    public void ShowAccountScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(), to: _accountScreen);
    }

    public void HideAccountScreen()
    {
        GoFromScreenToScreen(to: _homeScreen);
    }

    public void ShowConnectionFailedScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(), to: _connectionFailedScreen);
    }
    
    public void HideConnectionFailedScreen()
    {
        if(_connectionFailedScreen.ScreenBefore == null || _connectionFailedScreen.ScreenBefore is ConnectionFailedScreen)
            ShowHomeScreen();
        else
            GoFromScreenToScreen(to: _connectionFailedScreen.ScreenBefore);
    }

    private void ShowVersionFailedScreen()
    {
        GoFromScreenToScreen(to: _versionFailedScreen);
    }

    private void ShowOperationStatusScreen()
    {
        if(ActiveScreen() is ShopScreen)
            GoFromScreenToScreen(from: ActiveScreen() ,to: _operationStatusScreen);
    }
    
    public void HideOperationStatusScreen()
    {
        GoFromScreenToScreen(to: _operationStatusScreen.ScreenBefore);
    }

    private void ShowFinishScreen()
    {
        SetGameCameraActive();
        GoFromScreenToScreen(to: _finishScreen);
    }

    public void ShowTutorialScreen()
    {   
        SetGameCameraActive();
        GoFromScreenToScreen(to :_tutorialScreen);
    }

    public void ShowGameScreen()
    {
        SetGameCameraActive();
        GoFromScreenToScreen(to :_gameScreen);
    }

    public void ShowCodeInputScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(),to: _codeInputScreen);
    }

    public void HideCodeInputScreen()
    {
        GoFromScreenToScreen(to: _codeInputScreen.ScreenBefore);
    }
    
    public void ShowCustomizationScreen()
    {
        SetMenuCameraActive();
        GoFromScreenToScreen(from: ActiveScreen(),to: _customizationScreen);
    }

    public void HideCustomizationScreen()
    {
        SetGameCameraActive();
        GoFromScreenToScreen(to: _customizationScreen.ScreenBefore);
    }
    
    public void ShowSignOutScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(),to: _signOutScreen);
    }

    public void HideSignOutScreen()
    {
        GoFromScreenToScreen(to: _signOutScreen.ScreenBefore);
    }
    
    public void ShowLoadingScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(),to: _loadingScreen);
    }

    public void HideLoadingScreen()
    {
        GoFromScreenToScreen(to: _loadingScreen.ScreenBefore);
    }

    public void ShowLevelMenuScreen()
    {
        GoFromScreenToScreen(from: ActiveScreen(), to: _levelMenuScreen);
    }

    public void HideLevelMenuScreen()
    {
        GoFromScreenToScreen(to: _levelMenuScreen.ScreenBefore);
    }

    private void ShowGameOverAfterDie()
    {
        if(_connectionFailedScreen.IsVisible() == false && _versionFailedScreen.IsVisible() == false && _finishScreen.IsVisible() == false)
            _gameOverCoroutine = StartCoroutine(ShowGameOverAfter());
    }

    private void ShowGameAlert(string alert)
    {
        _gameAlertScreen.Alert = alert;
        GoFromScreenToScreen(to: _gameAlertScreen);
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
    
    private void CheckVersion(VersionFetch fetch)
    {
        if (fetch == VersionFetch.Old)
        {
            ShowVersionFailedScreen();
        }
    }
}
