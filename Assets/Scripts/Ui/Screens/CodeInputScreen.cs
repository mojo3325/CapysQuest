using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeInputScreen : MenuScreen
{

    public static event Action<string> EnterCodeClicked;
    
    private Button _backButton;
    private Button _codeEnterButton;
    private Button _success_ok_Button;

    private VisualElement root;
    private VisualElement _codeSuccessEnteredView;
    
    private TextField _codeTextField;
    private Label _topBarLabel;

    private static string _topBarLabelName = "code_top_bar_label";
    private static string _backButtonName = "BackButton";
    private static string _codeEnterButtonName = "CodeEnterButton";
    private static string _codeTextFieldName = "code_textfield";

    private static string codeHint = "Введите код";

    private string _enteredCode;

    [SerializeField] private DeviceType _deviceType;

    public override void ShowScreen()
    {
        base.ShowScreen();
        SetupFieldHint();
        HideCodeSuccessEntered();
    }

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        root = _root.Query<VisualElement>("CodeInputScreen");
        
        _topBarLabel = root.Q<Label>(_topBarLabelName);
        _codeTextField = root.Q<TextField>(_codeTextFieldName);
        _backButton = root.Q<Button>(_backButtonName);
        _codeEnterButton = root.Q<Button>(_codeEnterButtonName);
        
        _codeSuccessEnteredView = root.Q<VisualElement>("CodeSuccessView");
        _success_ok_Button = root.Q<Button>("ok_button");
    }

    private void Start()
    {
        _codeTextField.RegisterCallback<FocusInEvent>(OnCodeInputInFocus);
        _codeTextField.RegisterCallback<BlurEvent>(OnBlur);
        _codeTextField.RegisterValueChangedCallback(OnCodeValueChanged);
    }

    private void SetupFieldHint()
    {
        if(string.IsNullOrEmpty(_codeTextField.value))
            _codeTextField.value = codeHint;
    }

    private void OnCodeInputInFocus(FocusInEvent evt)
    {
        if (_codeTextField.value == codeHint)
            _codeTextField.value = "";
    }

    private void OnBlur(BlurEvent evt)
    {
        if (string.IsNullOrEmpty(_codeTextField.value))
            _codeTextField.value = codeHint;
    }

    private void OnCodeValueChanged(ChangeEvent<string> evt)
    {
        _enteredCode = evt.newValue;
        _codeTextField.value = _enteredCode;
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideCodeInputScreen();
            _codeTextField.value = codeHint;
        };

        _success_ok_Button.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            HideCodeSuccessEntered();
        };
        _codeEnterButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            EnterCode();
        };
    }

    private void ShowCodeSuccessEntered()
    {
        _codeSuccessEnteredView.style.display = DisplayStyle.Flex;
        _codeTextField.value = codeHint;
    }
    
    private void HideCodeSuccessEntered()
    {
        _codeSuccessEnteredView.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
        CodeInputController.CodeEnteredSuccess += ShowCodeSuccessEntered;
    }

    private void OnDisable()
    {
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
        CodeInputController.CodeEnteredSuccess -= ShowCodeSuccessEntered;
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
            _topBarLabel.style.fontSize = 60;
            _codeTextField.style.height = Length.Percent(15);
        }
        else
        {
            _topBarLabel.style.fontSize = 40;
            _codeTextField.style.height = Length.Percent(10);
        }
    }

    private void EnterCode()
    {
        EnterCodeClicked?.Invoke(_codeTextField.value.Trim());
    }
}

