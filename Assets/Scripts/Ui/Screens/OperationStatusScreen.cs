using System;
using System.Collections.Generic;
using System.IO;
using Google.MiniJSON;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class OperationStatusScreen : MenuScreen
{
    private VisualElement _loadCat;
    private Button _statusOkButton;
    private Label _statusText;

    private static string _statusCatName = "LoadCat";
    private static string _statusOkButtonName = "status_ok_buttton";
    private static string _statusTextName = "StatusText";
    private Dictionary<string, object> _gameStrings;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _loadCat = _root.Q<VisualElement>(_statusCatName);
        _statusOkButton = _root.Q<Button>(_statusOkButtonName);
        _statusText = _root.Q<Label>(_statusTextName);
        SetupSizes();
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _statusOkButton.clicked += OnOkButtonClick;
    }

    private void OnOkButtonClick()
    {
        _statusText.text = "";
        _mainMenuUIManager.HideOperationStatusScreen();
    }

    private void OnEnable()
    {
        ShopController.PurchaseCalled += SetupLoadingScreen;
        MenuManagerController.GameStringsInitialized += GameStringsInit;
    }

    private void GameStringsInit(Dictionary<string, object> strings)
    {
        _gameStrings = strings;
    }
    

    private void OnDisable()
    {
        ShopController.PurchaseCalled -= SetupLoadingScreen;
        MenuManagerController.GameStringsInitialized -= GameStringsInit;
    }
    
    private void SetupSizes()
    {
        var devicetype = Tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _statusText.style.fontSize = new StyleLength(60);
        }
        else if (devicetype == DeviceType.Tablet)
        {
            _statusText.style.fontSize = new StyleLength(45);
        }
    }

    private void SetupLoadingScreen(Status status)
    {
        if (status == Status.Failed)
            _statusText.text = _gameStrings["failed_operation"].ToString();
        if (status == Status.Success)
            _statusText.text = _gameStrings["successful_operation"].ToString();
    }
}
