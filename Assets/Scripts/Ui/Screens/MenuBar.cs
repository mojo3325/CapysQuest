using Assets.Scripts.Ui.Screens;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuBar : MenuScreen
{
    [SerializeField] private LevelMenuController levelMenuController;
    public static event Action<Level> PlayButtonClicked;
    public static event Action CustomizationButtonClicked;

    private Button _homePlayButton;
    private Button _gameOverPlayButton;
    
    private Button _homeSettingsButton;
    private Button _gameOverSettingsButton;
    
    private Button _shopButton;
    
    private Button _accountButton;
    private Button _customizationButton;
    
    private Button _homeButton;
    
    private VisualElement root;
    private VisualElement _homeMenuBar;
    private VisualElement _gameOverMenuBar;

    [SerializeField] private DeviceType _deviceType;
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        root = _root.Query<VisualElement>("MenuBar");

        _homeMenuBar = root.Query<VisualElement>("HomeMenuBar");
        _gameOverMenuBar = root.Query<VisualElement>("GameOverMenuBar");
        
        _homePlayButton = _homeMenuBar.Q<Button>("home_play_button");
        _homeSettingsButton = _homeMenuBar.Q<Button>("home_settings_button");
        _shopButton = _homeMenuBar.Q<Button>("home_shop_button");
        _customizationButton = _homeMenuBar.Q<Button>("home_customization_button");
        _accountButton = _homeMenuBar.Q<Button>("home_account_button");

        _gameOverPlayButton = _gameOverMenuBar.Q<Button>("gameOver_play_button");
        _gameOverSettingsButton = _gameOverMenuBar.Q<Button>("gameOver_settings_button");
        _homeButton = _gameOverMenuBar.Q<Button>("gameOver_home_button");
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        
        _homePlayButton.clicked += OnPlayClick;
        _gameOverPlayButton.clicked += OnPlayClick;
        _homeSettingsButton.clicked += OnSettingsClick;
        _gameOverSettingsButton.clicked += OnSettingsClick;
        _shopButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            _mainMenuUIManager.ShowShopScreen();
        };
        _accountButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            _mainMenuUIManager.ShowAccountScreen();
        };
        _customizationButton.clicked += () =>
        {
            CustomizationButtonClicked?.Invoke();
            ButtonEvent.OnOpenMenuCalled();
            _mainMenuUIManager.ShowCustomizationScreen();
        };
        _homeButton.clicked += () =>
        {
            ButtonEvent.OnOpenMenuCalled();
            _mainMenuUIManager.ShowHomeScreen();
        };
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
            _homePlayButton.style.width = Length.Percent(33);
            _homePlayButton.style.height = Length.Percent(50);
            
            _homeSettingsButton.style.height = Length.Percent(30);
            _accountButton.style.height = Length.Percent(30);
            _customizationButton.style.height = Length.Percent(30);
            _shopButton.style.height = Length.Percent(30);
            
            _gameOverPlayButton.style.height = Length.Percent(100);
        }
        if (_deviceType == DeviceType.Tablet)
        {
            _homePlayButton.style.height = Length.Percent(50);
            _homePlayButton.style.width = Length.Percent(40);
            
            _homeSettingsButton.style.height = Length.Percent(20);
            _accountButton.style.height = Length.Percent(20);
            _customizationButton.style.height = Length.Percent(20);
            _shopButton.style.height = Length.Percent(20);
            
            _gameOverPlayButton.style.height = Length.Percent(70);
            _gameOverPlayButton.style.width = Length.Percent(35);
        }
    }
    
    private void OnPlayClick()
    {
        ButtonEvent.OnOpenMenuCalled();
        
        var isTutorialAccepted = PlayerPrefs.GetInt("isTutorialAccepted", 0);

        if (isTutorialAccepted == 1)
        {
            if(_mainMenuUIManager.ActiveScreen() is GameOverScreen)
            {
                PlayButtonClicked?.Invoke(levelMenuController.currentLevel);
                _mainMenuUIManager.ShowGameScreen();
            }
                
            else
            {
                _mainMenuUIManager.ShowLevelMenuScreen();
            }
        }
        else if (isTutorialAccepted == 0)
        {
            _mainMenuUIManager.ShowTutorialScreen();
        }
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        var activeScreen = _mainMenuUIManager.ActiveScreen();

        if (activeScreen is not null)
        {
            if (activeScreen is GameOverScreen)
            {
                if (_homeMenuBar != null && _gameOverMenuBar != null)
                {
                    _homeMenuBar.style.display = DisplayStyle.None;
                    _gameOverMenuBar.style.display = DisplayStyle.Flex;    
                }
            }

            if (activeScreen is HomeScreen)
            {
                if (_homeMenuBar != null && _gameOverMenuBar != null)
                {
                    _gameOverMenuBar.style.display = DisplayStyle.None;
                    _homeMenuBar.style.display = DisplayStyle.Flex;    
                }
            }   
        }
    }

    private void OnSettingsClick()
    {
        ButtonEvent.OnOpenMenuCalled();
        _mainMenuUIManager.ShowSettingsScreen();
    }
    
    private void OnEnable()
    {
        ShowScreen();
        TutorialScreen.OnTutorialAccepted += OnPlayClick;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
        LevelMenuScreen.OnLevelPlayClick += (level) =>
        {
            PlayButtonClicked?.Invoke(levelMenuController.currentLevel);
        };
    }
    
    private void OnDisable()
    {
        TutorialScreen.OnTutorialAccepted -= OnPlayClick;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
        LevelMenuScreen.OnLevelPlayClick -= (level) =>
        {
            PlayButtonClicked?.Invoke(levelMenuController.currentLevel);
        };
    }
}
