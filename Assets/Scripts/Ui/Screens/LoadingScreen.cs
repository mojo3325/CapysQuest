using System;
using Assets.SimpleLocalization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class LoadingScreen : MenuScreen
{
    private VisualElement _loadCat;
    private Button _loadingOkButton;
    private Label _loadingText;

    private static string _loadCatName = "LoadCat";
    private static string _loadingOkButtonName = "LoadingOkButton";
    private static string _loadingTextName = "LoadingText";
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _loadCat = _root.Q<VisualElement>(_loadCatName);
        _loadingOkButton = _root.Q<Button>(_loadingOkButtonName);
        _loadingText = _root.Q<Label>(_loadingTextName);
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _loadingOkButton.clicked += OnOkButtonClick;
    }

    private void OnOkButtonClick()
    {
        _loadingText.text = "";
        _mainMenuUIManager.HideLoadingScreen();
    }

    private void OnEnable()
    {
        ShopController.PurchaseCalled += SetupLoadingScreen;
    }

    private void OnDisable()
    {
        ShopController.PurchaseCalled -= SetupLoadingScreen;
    }

    private void SetupLoadingScreen(TransactionStatus status)
    {
        if(status == TransactionStatus.Failed)
            _loadingText.text = LocalizationManager.Localize("failed_operation");
        else if(status == TransactionStatus.Success)
            _loadingText.text = LocalizationManager.Localize("successful_operation");
    }
}
