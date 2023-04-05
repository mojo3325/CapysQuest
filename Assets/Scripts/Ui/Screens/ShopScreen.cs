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
    private Button _restorePurchasesButton;

    private static string _backButtonName = "ShopBackButton";
    private static string _noAdsButtonName = "NoAdsButton";
    private static string _noAdsLabelName = "NoAdsLabel";
    private static string _restorePurchasesButtonName = "RestorePurchasesButton";

    private string RemoveAdsID = "com.piderstudio.capysquest.removeads";

    private Product _removeAdProduct;
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        _backButton = _root.Q<Button>(_backButtonName);
        _noAdsButton = _root.Q<Button>(_noAdsButtonName);
        _noAdsLabel = _root.Q<Label>(_noAdsLabelName);
        _restorePurchasesButton = _root.Q<Button>(_restorePurchasesButtonName);
    }
    
    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += _mainMenuUIManager.HideShopScreen;
        _noAdsButton.clicked += OnRemoveAdClick;
        _restorePurchasesButton.clicked += RestorePurchases;
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
        ShopController.ExtensionProviderInitialized += (_extensionProvider) =>  { this._extensionProvider = _extensionProvider; };

        SettingsController.LanguageChanged += SetupScreenInfo;

        ShopController.PurchaseCalled += OnPurchaseCalled;
    }

    private void OnDisable()
    {
        ShopController.StoreControllerInitialized -= (c) => SetupShopProducts(controller: c);
        ShopController.ExtensionProviderInitialized -= (_extensionProvider) =>  { this._extensionProvider = _extensionProvider; };

        SettingsController.LanguageChanged -= SetupScreenInfo;

        ShopController.PurchaseCalled -= OnPurchaseCalled;
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

    private void OnPurchaseCalled(TransactionStatus status)
    {
        if (status == TransactionStatus.Success)
            SetButtonInactive();
    }

    private void SetupShopProducts(IStoreController controller = null)
    {
        if (controller == null) return;
        
        _storeController = controller;
        _removeAdProduct = _storeController?.products.WithID(RemoveAdsID);
            
        var hasReceipt = _removeAdProduct.hasReceipt;
        var price = _removeAdProduct?.metadata.localizedPriceString;
        _noAdsButton.text = price;
            
        _noAdsButton.style.color = Color.white;
            
        if (hasReceipt)
        {
            SetButtonInactive();
        }
    }
    
    private void RestorePurchases()
    {
        if (_storeController != null && _extensionProvider != null) return;
        
        if (Application.platform is RuntimePlatform.IPhonePlayer)
        {
            var apple = _extensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((success, callback) => {});
        }
    }

    private void SetButtonInactive()
    {
        _noAdsButton.style.backgroundImage = new StyleBackground();
        _noAdsButton.style.height = new StyleLength(90);
        _noAdsButton.style.backgroundColor =  new StyleColor(new Color32(0xD3, 0xD1, 0xCE, 0xFF));;
        _noAdsButton.style.borderBottomLeftRadius = new StyleLength(15);
        _noAdsButton.style.borderBottomRightRadius = new StyleLength(15);
        _noAdsButton.style.borderTopLeftRadius = new StyleLength(15);
        _noAdsButton.style.borderTopRightRadius = new StyleLength(15);
    }
}
