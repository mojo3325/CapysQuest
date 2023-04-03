using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

public class ShopScreen : MenuScreen
{
    public static event Action<string> NoAdsButtonClicked;
    
    private Label _noAdsLabel;
    private Button _backButton;
    private Button _noAdsButton;

    private static string _backButtonName = "ShopBackButton";
    private static string _noAdsButtonName = "NoAdsButton";
    private static string _noAdsLabelName = "NoAdsLabel";
    
    private string RemoveAdsID = "com.piderstudio.capysquest.removeads";

    private Product _removeAdProduct;
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        _backButton = _root.Q<Button>(_backButtonName);
        _noAdsButton = _root.Q<Button>(_noAdsButtonName);
        _noAdsLabel = _root.Q<Label>(_noAdsLabelName);
    }
    
    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += OnBackButtonClicked;
        _noAdsButton.clicked += OnRemoveAdClick;
    }

    private void OnRemoveAdClick()
    {
        if (!_removeAdProduct.hasReceipt)
        {
            NoAdsButtonClicked?.Invoke(RemoveAdsID);
        }
    }

    private void OnEnable()
    {
        SetupScreenInfo();
        ShopController.StoreControllerInitialized += SetupShoproducts;
        SettingsController.LanguageChanged += SetupScreenInfo;
    }

    private void OnDisable()
    {
        ShopController.StoreControllerInitialized -= SetupShoproducts;
        SettingsController.LanguageChanged -= SetupScreenInfo;
    }

    private void SetupScreenInfo()
    {
        LocalizationManager.Read();
        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
        }
        _noAdsLabel.text = LocalizationManager.Localize("disableAd_label");
    }

    private void SetupShoproducts(IStoreController controller)
    {
        _removeAdProduct = controller.products.WithID(RemoveAdsID);
        var hasReceipt = _removeAdProduct.hasReceipt;
        
        var price = _removeAdProduct.metadata.localizedPriceString;
        
        _noAdsButton.style.color = Color.white;
        _noAdsButton.text = price;
        
        if (hasReceipt)
        {
            _noAdsButton.style.backgroundColor = Color.grey;
            _noAdsButton.styleSheets.Clear();
        }
    }
    
    private void OnBackButtonClicked()
    {
        if (ScreenBefore is HomeScreen || ScreenBefore == null)
            _mainMenuUIManager.ShowHomeScreen();
        if (ScreenBefore is GameOverScreen)
            StartCoroutine(_mainMenuUIManager.ShowGameOverAfter(0f));
    }
}
