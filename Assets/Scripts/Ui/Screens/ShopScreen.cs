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
    private IStoreController _storeController;
    
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
        _backButton.clicked += _mainMenuUIManager.HideShopScreen;
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
        ShopController.StoreControllerInitialized += (c) => SetupShopProducts(controller: c);
        SettingsController.LanguageChanged += SetupScreenInfo;

        ShopController.PurchaseCalled += (v) => SetupShopProducts(transactionStatus: v);
    }

    private void OnDisable()
    {
        ShopController.StoreControllerInitialized -= (c) => SetupShopProducts(controller: c);
        SettingsController.LanguageChanged -= SetupScreenInfo;
        
        ShopController.PurchaseCalled -= (v) => SetupShopProducts(transactionStatus: v);
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

    private void SetupShopProducts(IStoreController controller = null, TransactionStatus transactionStatus = TransactionStatus.Null)
    {
        if (controller == null) return;
        
        _storeController = controller;
        _removeAdProduct = _storeController?.products.WithID(RemoveAdsID);
            
        var hasReceipt = _removeAdProduct.hasReceipt;
        var price = _removeAdProduct?.metadata.localizedPriceString;
        _noAdsButton.text = price;
            
        _noAdsButton.style.color = Color.white;
            
        if (transactionStatus == TransactionStatus.Success)
        {
            SetButtonInactive();
        }
    }

    private void SetButtonInactive()
    {
        _noAdsButton.style.backgroundImage = new StyleBackground();
        _noAdsButton.style.height = new StyleLength(90);
        _noAdsButton.style.backgroundColor =  new StyleColor(new Color32(0xD3, 0xD1, 0xCE, 0xFF));;
        _noAdsButton.style.borderBottomLeftRadius = new StyleLength(20);
        _noAdsButton.style.borderBottomRightRadius = new StyleLength(20);
        _noAdsButton.style.borderTopLeftRadius = new StyleLength(20);
        _noAdsButton.style.borderTopRightRadius = new StyleLength(20);
    }
}
