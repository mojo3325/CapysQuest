using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class OperationStatusScreen : MenuScreen
{
    [SerializeField] private Sprite _sadCapy;
    [SerializeField] private Sprite _happyCapy;
    
    private VisualElement _loadCat;
    private Button _statusOkButton;
    private Label _statusText;

    private static string _statusCatName = "LoadCat";
    private static string _statusOkButtonName = "status_ok_buttton";
    private static string _statusTextName = "StatusText";
    private Dictionary<string, object> _gameStrings;
    private DeviceType _deviceType;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _loadCat = _root.Q<VisualElement>(_statusCatName);
        _statusOkButton = _root.Q<Button>(_statusOkButtonName);
        _statusText = _root.Q<Label>(_statusTextName);
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _statusOkButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            OnOkButtonClick();
        };
    }

    private void OnOkButtonClick()
    {
        _statusText.text = "";
        _mainMenuUIManager.HideOperationStatusScreen();
    }

    private void OnEnable()
    {
        ShopController.PurchaseCalled += SetupLoadingScreen;
        GameStringsController.GameStringsInitialized += GameStringsInit;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void GameStringsInit(Dictionary<string, object> strings)
    {
        _gameStrings = strings;
    }
    

    private void OnDisable()
    {
        ShopController.PurchaseCalled -= SetupLoadingScreen;
        GameStringsController.GameStringsInitialized -= GameStringsInit;
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
            _statusText.style.fontSize = new StyleLength(60);
        }
        else if (_deviceType == DeviceType.Tablet)
        {
            _statusText.style.fontSize = new StyleLength(45);
        }
    }

    private void SetupLoadingScreen(Status status)
    {
        if (status == Status.Failure)
        {
            _loadCat.style.backgroundImage = new StyleBackground(_sadCapy);
            _statusText.text = _gameStrings["failed_operation"].ToString();
        }

        if (status == Status.Success)
        {
            _loadCat.style.backgroundImage = new StyleBackground(_happyCapy);
            _statusText.text = _gameStrings["successful_operation"].ToString();
        }
    }
}
