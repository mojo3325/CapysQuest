using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreen: MenuScreen
{
    [SerializeField] private Sprite _sadCapy;
    [SerializeField] private Sprite _loadSprite;
    
    private VisualElement _loadingScreen;
    private VisualElement _loadIndicator;

    private Button _okButton;
    private Label _errorLabel;
    
    private static string _loadIndicatorName = "loading_indicator";
    private static string _loadingScreenName = "loading_screen";
    private static string _okButtonName= "loading_ok_button";
    private static string _errorLabelName= "loading_error_label";

    private Coroutine _loadAnimateCoroutine;

    private void OnEnable()
    {
        OperationEvent.MethodCalled += OnMethodCalled;
        OperationEvent.MethodFinished += OnMethodFinished;
    }

    private void OnDisable()
    {
        OperationEvent.MethodCalled -= OnMethodCalled;
        OperationEvent.MethodFinished -= OnMethodFinished;
    }

    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        _loadingScreen = _root.Query<VisualElement>(_loadingScreenName);

        _errorLabel = _loadingScreen.Q<Label>(_errorLabelName);
        _okButton = _loadingScreen.Q<Button>(_okButtonName);
        _loadIndicator = _loadingScreen.Q(_loadIndicatorName);
    }

    private void OnMethodCalled()
    { 
        _mainMenuUIManager.ShowLoadingScreen();
        
        _loadIndicator.style.backgroundImage = new StyleBackground(_loadSprite);
        _loadAnimateCoroutine = StartCoroutine(AnimateLoading());
    }

    private void OnMethodFinished(TaskResult<bool> task)
    {
        if (task.IsSuccess)
        {
            StopCoroutine(_loadAnimateCoroutine);
            _mainMenuUIManager.HideLoadingScreen();
        }
        else if (task.IsFailure)
        {
            StopCoroutine(_loadAnimateCoroutine);
            _loadIndicator.transform.rotation = Quaternion.Euler(0, 0, 0);
            _loadIndicator.style.backgroundImage = new StyleBackground(_sadCapy);
            _errorLabel.text = task.ErrorMessage;
            _okButton.style.display = DisplayStyle.Flex;
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        _okButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            _mainMenuUIManager.HideLoadingScreen();
            _okButton.style.display = DisplayStyle.None;
            _errorLabel.text = "";
            _loadIndicator.style.backgroundImage = new StyleBackground(_loadSprite);
        };
    }

    private IEnumerator AnimateLoading()
    {
        while (true)
        {
            var rotationAngle = 200f * Time.deltaTime;
            _loadIndicator.transform.rotation *= Quaternion.Euler(0, 0, rotationAngle);
            yield return null;
        }
    }
    
}
