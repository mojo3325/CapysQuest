using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SignOutScreen : MenuScreen
{
    [Header("Controllers")] [SerializeField]
    private AccountScreenController _accountScreenController;
    
    private VisualElement _signOutScreen;
    
    private Button _backButton;
    private Button _simple_signOutButton;
    private Button _signOut_delete_Button;
    
    private Label _simple_signOut_label;
    private Label _delete_signOut_label;
    
    private static string _backButtonName = "back_button";
    private DeviceType _deviceType;
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _signOutScreen = _root.Q<VisualElement>("SignOutScreen");
        
        _backButton = _root.Q<Button>(_backButtonName);
        _simple_signOutButton = _root.Q<Button>("simple_signOut_button");
        _signOut_delete_Button = _root.Q<Button>("delete_and_signOut_button");
        
        _simple_signOut_label = _root.Q<Label>("simple_signOut_label"); 
        _delete_signOut_label =  _root.Q<Label>("delete_signOut_label");
        
        _showMenuBar = false;
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideSignOutScreen();
        };
        
        _simple_signOutButton.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            SignOut();
        };
        _signOut_delete_Button.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            SignOutAndDelete();
        };
    }
    
    private void SignOut()
    {
        _accountScreenController.SignOut();
        _mainMenuUIManager.ShowAccountScreen();
    }

    private async void SignOutAndDelete()
    {
        await _accountScreenController.DeleteAccount();
        _mainMenuUIManager.ShowAccountScreen();
    }

    
    private void OnEnable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void OnDisable()
    {
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
            _simple_signOutButton.style.height = Length.Percent(20);
            _signOut_delete_Button.style.height = Length.Percent(20);
            
            _simple_signOut_label.style.fontSize = 33; 
            _delete_signOut_label.style.fontSize = 33;
        }
        else
        {

            _simple_signOutButton.style.height = Length.Percent(10);
            _signOut_delete_Button.style.height = Length.Percent(10);
            
            _simple_signOut_label.style.fontSize = 25; 
            _delete_signOut_label.style.fontSize = 25;
        }
    }
}
