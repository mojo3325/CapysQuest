using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Google;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AccountScreen : MenuScreen
{

    [Header("Controllers")] [SerializeField]
    private AccountScreenController _accountScreenController;

    private VisualElement _loadingScreen;
    private VisualElement _accountScreen;
    
    private VisualElement _signUpContainer;
    private VisualElement _signInContainer;
    private VisualElement _refCodeContainer;
    private VisualElement _signUpLabelContainer;
    private VisualElement _signInLabelContainer;

    private VisualElement _welcomeScreen;
    private VisualElement _accountInfoScreen;
    
    private Button _backButton;

    private TextField _emailTextField;
    private TextField _passwordTextField;
    private TextField _refCodeTextField;
    
    private Label _topBarLabel;
    private Label _refButtonCodeLabel;

    private Label _dontHaveAccountLabel;
    private Label _haveAnAccountLabel;
    
    private Label _refCodeLabel;
    private Label _finishPrizeLabel;
    
    private Label _finishPrizeValue;
    private Label _signInWithLabel;

    private Button _signInButton;
    private Button _android_googleSignIn_Button;
    private Button _ios_googleSignIn_Button;
    private Button _ios_appleSignIn_Button;
    private Button _signUpLabelButton;
    private Button _signInLabelButton;
    private Button _signUpButton;
    private Button _signOutButton;
    private Button _refCodeEnterButton;
    private Button _refCodeSkipButton;
    private Button _refCodeCopyButton;
    
    private static string _signUpContainerName = "sign_up_container";
    private static string _signInContainerName = "sign_in_container";
    private static string _refCodeContainerName = "ref_code_container";
    private static string _signUpLabelContainerName = "sign_up_label_container";
    private static string _signInLabelContainerName = "sign_in_label_container";
    
    private static string _welcomeScreenName = "WelcomeScreen";
    private static string _accountInfoScreenName = "AccountInfoScreen";
    
    private static string _backButtonName = "account_back_button";
    private static string _signUpLabelButtonName = "sign_up_label_button";
    private static string _signInLabelButtonName = "sign_in_label_button";
    private static string _signInButtonName = "SignInButton";
    private static string _refCodeEnterButtonName = "RefCodeEnter";
    private static string _refCodeSkipButtonName = "RefCodeSkip";
    private static string _android_googleSignIn_ButtonName = "android_google_sign_in";
    private static string _ios_googleSignIn_ButtonName = "ios_google_sign_in";
    private static string _ios_appleSignIn_ButtonName = "ios_apple_sign_in";
    private static string _signUpButtonName = "SignUpButton";
    private static string _signOutButtonName = "account_signOut_button";
    private static string _refCodeCopyButtonName = "referral_code_button";
    
    private static string _emailTextFieldName = "EmailField";
    private static string _passwordTextFieldName = "PasswordField";
    private static string _refCodeTextFieldName = "RefCodeField";
    
    private static string _topBarLabelName = "account_top_bar_label";
    private static string _refButtonCodeLabelName = "referral_code";
    
    private static string _refCodeLabelName = "ref_code_label";
    private static string _finisPrizeLabelName = "finish_prize_label";
    
    private static string _finisPrizeValueName = "finish_prize_value";
    private static string _signWithLabelName = "sign_with_label";
    
    private static string emailHint = "Enter your mail";
    private static string passwordHint = "Enter your password";
    private static string refCodedHint = "Enter referral code";
    
    private string _enteredEmail;
    private string _enteredPassword;
    private string _enteredRefCode;
    
    private Coroutine _copyMessageCoroutine;
    private DeviceType _deviceType;

    public VisualElement ScreenRoot => _root;

    protected override void SetVisualElements()
      {
       base.SetVisualElements();
       _showMenuBar = false;
       
       _accountScreen = _root.Q<VisualElement>("account_screen");
       _loadingScreen = _root.Q<VisualElement>("loading_screen");
       
       _emailTextField = _root.Q<TextField>(_emailTextFieldName);
       
       _passwordTextField = _root.Q<TextField>(_passwordTextFieldName);
       _refCodeTextField = _root.Q<TextField>(_refCodeTextFieldName);
       
       _topBarLabel = _root.Q<Label>(_topBarLabelName);
       _refButtonCodeLabel = _root.Q<Label>(_refButtonCodeLabelName);

       _refCodeLabel = _root.Q<Label>(_refCodeLabelName);
       _finishPrizeLabel = _root.Q<Label>(_finisPrizeLabelName);

       _finishPrizeValue = _root.Q<Label>(_finisPrizeValueName);

       _signInWithLabel = _root.Q<Label>(_signWithLabelName);

       _dontHaveAccountLabel = _root.Q<Label>("dont_have_account_label");
       _haveAnAccountLabel = _root.Q<Label>("have_an_account_label");
       
       _signInButton = _root.Q<Button>(_signInButtonName);
       _signUpButton = _root.Q<Button>(_signUpButtonName);
       _android_googleSignIn_Button = _root.Q<Button>(_android_googleSignIn_ButtonName);
       _ios_googleSignIn_Button = _root.Q<Button>(_ios_googleSignIn_ButtonName);
       _ios_appleSignIn_Button = _root.Q<Button>(_ios_appleSignIn_ButtonName);
       
       _signUpLabelButton = _root.Q<Button>(_signUpLabelButtonName);
       _signInLabelButton = _root.Q<Button>(_signInLabelButtonName);
       _backButton = _root.Q<Button>(_backButtonName);
       _signOutButton = _root.Q<Button>(_signOutButtonName);
       _refCodeEnterButton = _root.Q<Button>(_refCodeEnterButtonName);
       _refCodeSkipButton = _root.Q<Button>(_refCodeSkipButtonName);
       _refCodeCopyButton = _root.Q<Button>(_refCodeCopyButtonName);

       _signInContainer = _root.Q<VisualElement>(_signInContainerName);
       _signUpContainer = _root.Q<VisualElement>(_signUpContainerName);
       _signUpLabelContainer = _root.Q<VisualElement>(_signUpLabelContainerName);
       _signInLabelContainer = _root.Q<VisualElement>(_signInLabelContainerName);
       _refCodeContainer = _root.Q<VisualElement>(_refCodeContainerName);
       
       _welcomeScreen = _root.Q<VisualElement>(_welcomeScreenName);
       _accountInfoScreen = _root.Q<VisualElement>(_accountInfoScreenName);

       SetupSignInContainer();
      }

    private void OnEnable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
        FireBaseController.FireBaseRepoInitialized += UpdateConfig;
    }

    private void OnDisable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
        FireBaseController.FireBaseRepoInitialized -= UpdateConfig;
    }
    
    private void SetupFieldsHint()
    {
        if(string.IsNullOrEmpty(_emailTextField.value))
            _emailTextField.value = emailHint;
        if(string.IsNullOrEmpty(_passwordTextField.value))
            _passwordTextField.value = passwordHint;
        if(string.IsNullOrEmpty(_refCodeTextField.value))
            _refCodeTextField.value = refCodedHint;
    }
    
    private void OnAuthenticationChecked(bool isAuthenticated)
    {
        if (isAuthenticated)
        {   
            OnUserAuthenticated();
        }

        if (!isAuthenticated)
        {
            OnUserNotAuthenticated();
        }
    }

    private void Start()
      {
          _emailTextField.RegisterCallback<FocusInEvent>(OnEmailInFocus);
          _passwordTextField.RegisterCallback<FocusInEvent>(OnPasswordInFocus);
          _refCodeTextField.RegisterCallback<FocusInEvent>(OnRefCodedInFocus);
          _emailTextField.RegisterCallback<BlurEvent>(OnBlur);
          _passwordTextField.RegisterCallback<BlurEvent>(OnBlur);
          _refCodeTextField.RegisterCallback<BlurEvent>(OnBlur);
          
          _emailTextField.RegisterValueChangedCallback(OnEmailValueChanged);
          _passwordTextField.RegisterValueChangedCallback(OnPasswordValueChanged);
          _refCodeTextField.RegisterValueChangedCallback(OnRefCodeValueChanged);
      }
      
      private void OnEmailValueChanged(ChangeEvent<string> evt)
      {
          _enteredEmail = evt.newValue;
          _emailTextField.value = _enteredEmail;
      }
      
      private void OnPasswordValueChanged(ChangeEvent<string> evt)
      {
          _enteredPassword = evt.newValue;
          _passwordTextField.value = _enteredPassword;
      }

      private void OnRefCodeValueChanged(ChangeEvent<string> evt)
      {
          _enteredRefCode = evt.newValue;
          _refCodeTextField.value = _enteredRefCode;
      }

      private void SetupSignInContainer()
      {
          if (Application.platform == RuntimePlatform.Android)
          {
              _signInWithLabel.text = "or";
              _android_googleSignIn_Button.style.display = DisplayStyle.Flex;
              _ios_googleSignIn_Button.style.display = DisplayStyle.None;
              _ios_appleSignIn_Button.style.display = DisplayStyle.None;
          }
          else if (Application.platform == RuntimePlatform.IPhonePlayer)
          {
              _signInWithLabel.text = "Or log in via:";
              _android_googleSignIn_Button.style.display = DisplayStyle.None;
              _ios_googleSignIn_Button.style.display = DisplayStyle.Flex;
              _ios_appleSignIn_Button.style.display = DisplayStyle.Flex;
          }
          else
          {
              _signInWithLabel.text = "Or log in via:";
              _android_googleSignIn_Button.style.display = DisplayStyle.None;
              _ios_googleSignIn_Button.style.display = DisplayStyle.Flex;
              _ios_appleSignIn_Button.style.display = DisplayStyle.Flex;
          }
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
              _topBarLabel.style.fontSize = 40;
              _signInButton.style.fontSize = 40;
              _refButtonCodeLabel.style.fontSize = 30;
              _refCodeEnterButton.style.fontSize = 40;
              _refCodeSkipButton.style.fontSize = 40;
              _signUpLabelButton.style.fontSize = 40;
              _signInLabelButton.style.fontSize = 40;
              _signUpButton.style.fontSize = 30;
              _emailTextField.style.height = Length.Percent(25);
              _passwordTextField.style.height = Length.Percent(25);
              _emailTextField.style.fontSize = 35;
              _passwordTextField.style.fontSize = 35;

              _dontHaveAccountLabel.style.fontSize = 35;
              _haveAnAccountLabel.style.fontSize = 35;

              _refCodeLabel.style.fontSize = 45;
              _finishPrizeLabel.style.fontSize = 45;

              _finishPrizeValue.style.fontSize = 55;

              _signInWithLabel.style.fontSize = 35;

              _ios_googleSignIn_Button.style.width = Length.Percent(8);
              _ios_appleSignIn_Button.style.width = Length.Percent(8);

              _emailTextField.AddToClassList("phone-text-field");
              _passwordTextField.AddToClassList("phone-text-field");
              _refCodeTextField.AddToClassList("phone-text-field");
          }
          else
          {
              _topBarLabel.style.fontSize = 35;
              _signInButton.style.fontSize = 30;
              _refButtonCodeLabel.style.fontSize = 20;
              _refCodeEnterButton.style.fontSize = 30;
              _refCodeSkipButton.style.fontSize = 30;
              _signUpLabelButton.style.fontSize = 25;
              _signInLabelButton.style.fontSize = 25;
              _signUpButton.style.fontSize = 20;
              _emailTextField.style.height = Length.Percent(18);
              _passwordTextField.style.height = Length.Percent(18);
              _emailTextField.style.fontSize = 25;
              _passwordTextField.style.fontSize = 25;
              
              _dontHaveAccountLabel.style.fontSize = 25;
              _haveAnAccountLabel.style.fontSize = 25;
              
              _refCodeLabel.style.fontSize = 30;
              _finishPrizeLabel.style.fontSize = 30;
              
              _finishPrizeValue.style.fontSize = 40;
              
              _signInWithLabel.style.fontSize = 25;
              
              _ios_googleSignIn_Button.style.width = Length.Percent(12);
              _ios_appleSignIn_Button.style.width = Length.Percent(12);

              _emailTextField.AddToClassList("tablet-text-field");
              _passwordTextField.AddToClassList("tablet-text-field");
              _refCodeTextField.AddToClassList("tablet-text-field");
          }
      }
      
        private void OnEmailInFocus(FocusInEvent evt)
        {
            if (string.Equals(_emailTextField.value, emailHint))
                _emailTextField.value = string.Empty;
        }

        private void OnPasswordInFocus(FocusInEvent evt)
        {
            if (string.Equals(_passwordTextField.value, passwordHint))
                _passwordTextField.value = string.Empty;
        }

        private void OnRefCodedInFocus(FocusInEvent evt)
        {
            if (string.Equals(_refCodeTextField.value, refCodedHint))
                _refCodeTextField.value = string.Empty;
        }


      private void OnBlur(BlurEvent evt)
      {
          if (string.IsNullOrEmpty(_emailTextField.value))
              _emailTextField.value = emailHint;
          
          if (string.IsNullOrEmpty(_passwordTextField.value))
              _passwordTextField.value = passwordHint;

          if (string.IsNullOrEmpty(_refCodeTextField.value))
              _refCodeTextField.value = refCodedHint;
      }

      private void ShowSignUpMenu()
      {
          _accountInfoScreen.style.display = DisplayStyle.None;
          _welcomeScreen.style.display = DisplayStyle.Flex;

          _signInContainer.style.display = DisplayStyle.None;
          _signUpContainer.style.display = DisplayStyle.Flex;
          _refCodeContainer.style.display = DisplayStyle.None;
          
          _signUpLabelContainer.style.display = DisplayStyle.None;
          _signInLabelContainer.style.display = DisplayStyle.Flex;
          
          _emailTextField.style.display =  DisplayStyle.Flex;
          _passwordTextField.style.display =  DisplayStyle.Flex;
          _refCodeTextField.style.display =  DisplayStyle.None;
      }
      
      private void ShowSignInMenu()
      {
          _accountInfoScreen.style.display = DisplayStyle.None;
          _welcomeScreen.style.display = DisplayStyle.Flex;

          _signInContainer.style.display = DisplayStyle.Flex;
          _signUpContainer.style.display = DisplayStyle.None;
          _refCodeContainer.style.display = DisplayStyle.None;
          
          _signUpLabelContainer.style.display = DisplayStyle.Flex;
          _signInLabelContainer.style.display = DisplayStyle.None;
          
          _emailTextField.style.display =  DisplayStyle.Flex;
          _passwordTextField.style.display =  DisplayStyle.Flex;
          _refCodeTextField.style.display =  DisplayStyle.None;
      }

      private void ShowRefCodeEnterMenu()
      {
          _accountInfoScreen.style.display = DisplayStyle.None;
          _welcomeScreen.style.display = DisplayStyle.Flex;
          
          _signUpLabelContainer.style.display = DisplayStyle.None;
          _signInLabelContainer.style.display = DisplayStyle.None;
          _signInContainer.style.display = DisplayStyle.None;
          _signUpContainer.style.display = DisplayStyle.None;
          _emailTextField.style.display =  DisplayStyle.None;
          _passwordTextField.style.display =  DisplayStyle.None;
          
          _refCodeContainer.style.display = DisplayStyle.Flex;
          _refCodeTextField.style.display =  DisplayStyle.Flex;
      }

      protected override void RegisterButtonCallbacks()
      {
          base.RegisterButtonCallbacks();
          _backButton.clicked += OnBackClicked;
          _signUpLabelButton.clicked += ShowSignUpMenu;
          _signInLabelButton.clicked += ShowSignInMenu;
          _signInButton.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              SignIn();
          };
          _android_googleSignIn_Button.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              GoogleSignIn();
          };
          _ios_googleSignIn_Button.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              GoogleSignIn();
          };
          _ios_appleSignIn_Button.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              AppleSignIn();
          };
          _signUpButton.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              SignUp();
          };
          _signOutButton.clicked += () =>
          {
              ButtonEvent.OnOpenMenuCalled();
              OpenSignOutMenu();
          };
          _refCodeEnterButton.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              RefCodeEnter();
          };
          _refCodeSkipButton.clicked += () =>
          {
              ButtonEvent.OnOpenMenuCalled();
              RefCodeSkip();
          };
          _refCodeCopyButton.clicked += () =>
          {
              ButtonEvent.OnEnterButtonCalled();
              OnRefCodeCopyClick();
          };
      }
      
      private void UpdateConfig()
      {
         var task = _accountScreenController.CheckAuth();
         OnAuthenticationChecked(task);
      }

      private async void SignIn()
      {
          var task = await _accountScreenController.SignInWithEmailAndPassword(_enteredEmail.Trim(), _enteredPassword.Trim());
          
          if (task.IsSuccess)
          {
              if (string.IsNullOrEmpty(_accountScreenController.CurrentUser.enteredReferralCode))
              {
                  ShowRefCodeEnterMenu();
              }
              else
              {
                  UpdateConfig();
              }
          }
      }

      public override void ShowScreen()
      {
          base.ShowScreen();
          UpdateConfig();
      }

      private void OnBackClicked()
      {
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideAccountScreen();
            _emailTextField.value = emailHint;
            _passwordTextField.value = passwordHint;
      }

      private async void GoogleSignIn()
      {
          var task = await _accountScreenController.SignInWithGoogle();
          
          if(task.IsSuccess)
          {
              if (string.IsNullOrEmpty(_accountScreenController.CurrentUser.enteredReferralCode))
              {
                  ShowRefCodeEnterMenu();
              }
              else
              {
                  UpdateConfig();
              }              
          }
      }
      
      private async void AppleSignIn()
      {
          var task = await _accountScreenController.SignInWithApple();
          
          if(task.IsSuccess)
          {
              if (string.IsNullOrEmpty(_accountScreenController.CurrentUser.enteredReferralCode))
              {
                  ShowRefCodeEnterMenu();
              }
              else
              {
                  UpdateConfig();
              }              
          }
      }

      private async void SignUp()
      {
          var task = await _accountScreenController.SignUpWithEmailAndPassword(_enteredEmail.Trim(), _enteredPassword.Trim());

          if (task.IsSuccess)
          {
              ShowRefCodeEnterMenu();
          }
      }
      
      private void OpenSignOutMenu()
      {
            _mainMenuUIManager.ShowSignOutScreen();
      }

      private async void RefCodeEnter()
      {
          var task = await _accountScreenController.EnterRefCode(code: _refCodeTextField.value.Trim());

          if (task.IsSuccess)
          {
              UpdateConfig();
          }

          if (task.IsFailure)
          {
          }
      }
      
      private void RefCodeSkip()
      {
          UpdateConfig();
      } 

      private async void OnUserAuthenticated()
      {
          _accountInfoScreen.style.display = DisplayStyle.Flex;
          _welcomeScreen.style.display = DisplayStyle.None;
          
          _signOutButton.style.visibility = Visibility.Visible;

          if (_accountScreenController.CurrentUser != null)
          {
              var userID = _accountScreenController.CurrentUser.userID;
              var userReferralCode = _accountScreenController.CurrentUser.referralCode;
          
              _topBarLabel.text = userID;
              _refButtonCodeLabel.text = userReferralCode;
              _finishPrizeValue.text = _accountScreenController.FinishPrize.ToString();

          }

          var task = await _accountScreenController.GetUserDetails();
          
          if (task == Status.Success)
          {
              _topBarLabel.text = _accountScreenController.CurrentUser.userID;
              _refButtonCodeLabel.text = _accountScreenController.CurrentUser.referralCode;
              _finishPrizeValue.text = _accountScreenController.FinishPrize.ToString();
          }
      }
      
      private void OnUserNotAuthenticated()
      {
          _signOutButton.style.visibility = Visibility.Hidden;
          _topBarLabel.text = "ACCOUNT";
          _refButtonCodeLabel.text = "";
          
          SetupFieldsHint();
          ShowSignInMenu();
      }

      private void OnRefCodeCopyClick()
      {
          if (_copyMessageCoroutine != null)
          {
              StopCoroutine(_copyMessageCoroutine);
          }

          _copyMessageCoroutine = StartCoroutine(CopyMessage());
      }

      private IEnumerator CopyMessage()
      {
          if(!string.IsNullOrEmpty(_accountScreenController.CurrentUser.referralCode))
              GUIUtility.systemCopyBuffer = _accountScreenController.CurrentUser.referralCode;
          if(_deviceType == DeviceType.Phone)
            _refButtonCodeLabel.style.fontSize = 25;
          if(_deviceType == DeviceType.Tablet) 
              _refButtonCodeLabel.style.fontSize = 15;
          _refButtonCodeLabel.text = "COPIED";
          yield return new WaitForSeconds(1f);
          if(_deviceType == DeviceType.Phone)
              _refButtonCodeLabel.style.fontSize = 30;
          if(_deviceType == DeviceType.Tablet) 
              _refButtonCodeLabel.style.fontSize = 20;
          _refButtonCodeLabel.text = _accountScreenController.CurrentUser.referralCode;
      }
}


