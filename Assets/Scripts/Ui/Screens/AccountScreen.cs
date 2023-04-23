using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class AccountScreen : MenuScreen
{
    private VisualElement _signUpContainer;
    private VisualElement _signInContainer;
    private VisualElement _signUpLabelContainer;
    private VisualElement _signInLabelContainer;
    
    private Button _backButton;
    
    private TextField _emailTextField;
    private TextField _passwordTextField;

    private Button _signInButton;
    private Button _signUpLabelButton;
    private Button _signInLabelButton;
    private Button _signUpButton;
    private Button _signOutButton;
    
    private static string _signUpContainerName = "SignUpContainer";
    private static string _signInContainerName = "SignInContainer";
    private static string _signUpLabelContainerName = "sign_up_label_container";
    private static string _signInLabelContainerName = "sign_in_label_container";
    
    private static string _backButtonName = "account_back_button";
    private static string _signUpLabelButtonName = "sign_up_label_button";
    private static string _signInLabelButtonName = "sign_in_label_button";
    private static string _signInButtonName = "SignInButton";
    private static string _signUpButtonName = "SignUpButton";
    
    private static string _emailTextFieldName = "EmailField";
    private static string _passwordTextFieldName = "PasswordField";
    
    private static string emailHint = "Введите почту";
    private static string passwordHint = "Введите пароль";
    
    private StyleSheet styleSheet;
    private StyleLength activeBorderWidth;
    private StyleColor activeBorderColor;
    private StyleLength inactiveBorderWidth;
    private StyleColor inactiveBorderColor;

    private string _enteredEmail;
    private string _enteredPassword;

    protected override void SetVisualElements()
      {
       base.SetVisualElements();
       _showMenuBar = false;
       
       _emailTextField = _root.Q<TextField>(_emailTextFieldName);
       _passwordTextField = _root.Q<TextField>(_passwordTextFieldName);
       
       _signInButton = _root.Q<Button>(_signInButtonName);
       _signUpButton = _root.Q<Button>(_signUpButtonName);
       _signUpLabelButton = _root.Q<Button>(_signUpLabelButtonName);
       _signInLabelButton = _root.Q<Button>(_signInLabelButtonName);
       _backButton = _root.Q<Button>(_backButtonName);

       _signInContainer = _root.Q<VisualElement>(_signInContainerName);
       _signUpContainer = _root.Q<VisualElement>(_signUpContainerName);
       _signUpLabelContainer = _root.Q<VisualElement>(_signUpLabelContainerName);
       _signInLabelContainer = _root.Q<VisualElement>(_signInLabelContainerName);
       
       SetupSizes();
      }
      
      private void Start()
      {
          _emailTextField.value = emailHint;
          _passwordTextField.value = passwordHint;
          _emailTextField.RegisterCallback<FocusInEvent>(OnEmailInFocus);
          _passwordTextField.RegisterCallback<FocusInEvent>(OnPasswordInFocus);
          _emailTextField.RegisterCallback<BlurEvent>(OnBlur);
          _passwordTextField.RegisterCallback<BlurEvent>(OnBlur);
          _emailTextField.RegisterValueChangedCallback(OnEmailValueChanged);
      }
      
      private void OnEmailValueChanged(ChangeEvent<string> evt)
      {
          _enteredEmail = evt.newValue;
          _emailTextField.value = _enteredEmail;
      }

      private void SetupSizes()
      {
          var devicetype = Tools.GetDeviceType();

          if (devicetype == DeviceType.Phone)
          {
              _signInButton.style.fontSize = 45;
              _signUpLabelButton.style.fontSize = 40;
              _signInLabelButton.style.fontSize = 40;
              _signUpButton.style.fontSize = 30;
          }
          else
          {
              _signInButton.style.fontSize = 35;
              _signUpLabelButton.style.fontSize = 30;
              _signInLabelButton.style.fontSize = 30;
              _signUpButton.style.fontSize = 20;
          }
      }
      
      private void OnEmailInFocus(FocusInEvent evt)
      {
          if (_emailTextField.value == emailHint)
              _emailTextField.value = "";
      }
      
      private void OnPasswordInFocus(FocusInEvent evt)
      {
          if (_passwordTextField.value == passwordHint)
              _passwordTextField.value = "";
      }

      private void OnBlur(BlurEvent evt)
      {
          if (string.IsNullOrEmpty(_emailTextField.value))
              _emailTextField.value = emailHint;
          
          if (string.IsNullOrEmpty(_passwordTextField.value))
              _passwordTextField.value = passwordHint;
      }

      private void ShowSignUpMenu()
      {
          _signInContainer.style.display = DisplayStyle.None;
          _signUpContainer.style.display = DisplayStyle.Flex;
          
          _signUpLabelContainer.style.display = DisplayStyle.None;
          _signInLabelContainer.style.display = DisplayStyle.Flex;
      }
      
      private void ShowSignInMenu()
      {
          _signInContainer.style.display = DisplayStyle.Flex;
          _signUpContainer.style.display = DisplayStyle.None;
          
          _signUpLabelContainer.style.display = DisplayStyle.Flex;
          _signInLabelContainer.style.display = DisplayStyle.None;
      }
      
      protected override void RegisterButtonCallbacks()
      {
          base.RegisterButtonCallbacks();
          _backButton.clicked += _mainMenuUIManager.HideAccountScreen;
          _signUpLabelButton.clicked += ShowSignUpMenu;
          _signInLabelButton.clicked += ShowSignInMenu;
      }
}

