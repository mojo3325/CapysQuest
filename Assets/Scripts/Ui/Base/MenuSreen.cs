using UnityEngine;
using UnityEngine.UIElements;
using System;

public abstract class MenuScreen : MonoBehaviour
{
    public static event Action ScreenShown;

    [SerializeField] protected string _screenName;
    [SerializeField] protected MainMenuUIManager _mainMenuUIManager;
    [SerializeField] protected UIDocument _document;
    private MenuScreen _screenBefore;
    protected bool _showMenuBar = false;
    private VisualElement _screen;
    protected VisualElement _root;

    public MenuScreen ScreenBefore { get => _screenBefore; set => _screenBefore = value; }
    public bool ShowMenuBar { get => _showMenuBar; set => _showMenuBar = value; }

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(_screenName))
            _screenName = this.GetType().Name;
    }

    protected virtual void Awake()
    {
        if (_mainMenuUIManager == null)
            _mainMenuUIManager = GetComponent<MainMenuUIManager>();

        if (_document == null)
            _document = GetComponent<UIDocument>();

        if (_document == null && _mainMenuUIManager != null)
            _document = _mainMenuUIManager.MainMenuDocument;

        if (_document == null)
        {
            Debug.LogWarning("MenuScreen " + _screenName + ": missing UIDocument. Check Script Execution Order.");
            return;
        }
        else
        {
            SetVisualElements();
            RegisterButtonCallbacks();
        }
    }

    protected virtual void SetVisualElements()
    {
        if (_document != null)
            _root = _document.rootVisualElement;

        _screen = GetVisualElement(_screenName);
    }

    protected virtual void RegisterButtonCallbacks()
    {

    }


    public bool IsVisible()
    {
        if (_screen == null)
             return false;
       
        return (_screen.style.display == DisplayStyle.Flex);
    }

    public static void ShowVisualElement(VisualElement visualElement, bool state)
    {
        if (visualElement == null)
            return;

        visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public VisualElement GetVisualElement(string elementName)
    {
        if (string.IsNullOrEmpty(elementName) || _root == null)
            return null;

        return _root.Q(elementName);
    }

    public virtual void ShowScreen()
    {
        ShowVisualElement(_screen, true);
        ScreenShown?.Invoke();
    }

    public virtual void HideScreen()
    {
        if (IsVisible())
        {
            ShowVisualElement(_screen, false);
        }
    }
}

