using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

public class ShopScreen : MenuScreen
{
    public static event Action<string> NoAdsButtonClicked;
    
    // private Label _2shopItemLabel;
    // private Label _3shopItemLabel;
    
    private Button _backButton;
    private Button _noAdsButton;
    private Label _noAdsLabelPrice;
    private Button _restorePurchasesButton;
    
    private Label _topBarLabel;

    private static string _backButtonName = "ShopBackButton";
    private static string _noAdsButtonName = "NoAdsButton";
    private static string _noAdsLabelPriceName = "remove_ads_price_label";
    private static string _topBarLabelName = "shop_top_bar_label";

    // private static string _2shopItemLabelName = "2shopItemLabel";
    // private static string _3shopItemLabelName = "3shopItemLabel";

    private static string _restorePurchasesButtonName = "RestorePurchasesButton";

    #if UNITY_IOS
        private string RemoveAdsID = "com.PiderStudio.CapysQuest.RemoveAdvertisement";
    #elif UNITY_ANDROID 
        private string RemoveAdsID = "com.piderstudio.capysquest.removeadvertisement";  
    #endif
    
    private Product _removeAdProduct;
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;
    private DeviceType _deviceType;

    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        _topBarLabel = _root.Q<Label>(_topBarLabelName);
        _backButton = _root.Q<Button>(_backButtonName);
        _noAdsButton = _root.Q<Button>(_noAdsButtonName);
        _noAdsLabelPrice = _root.Q<Label>(_noAdsLabelPriceName);
        _restorePurchasesButton = _root.Q<Button>(_restorePurchasesButtonName);
        // _2shopItemLabel = _root.Q<Label>(_2shopItemLabelName);
        // _3shopItemLabel = _root.Q<Label>(_3shopItemLabelName);
    }

    private void SetupScreenSize()
    {
        _deviceType = _mainMenuUIManager.DeviceController.DeviceType;

        if (string.IsNullOrEmpty(_deviceType.ToString()))
        {
            _mainMenuUIManager.DeviceController.SyncDeviceType();
            _deviceType = _mainMenuUIManager.DeviceController.DeviceType;
        }


        if (_deviceType == DeviceType.Phone)
        {
            _noAdsButton.style.width = Length.Percent(25);
            _topBarLabel.style.fontSize = new StyleLength(60);
        }
        
        if (_deviceType == DeviceType.Tablet)
        {
            _noAdsButton.style.width = Length.Percent(35);
            _topBarLabel.style.fontSize = new StyleLength(40);
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _backButton.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideShopScreen();
        };
        _noAdsButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            OnRemoveAdClick();
        };
        _restorePurchasesButton.clicked += () =>
        {
            ButtonEvent.OnEnterButtonCalled();
            RestorePurchases();
        };
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
        
        ShopController.StoreControllerInitialized += SetupShopProducts;
        ShopController.ExtensionProviderInitialized += (_extensionProvider) =>  { this._extensionProvider = _extensionProvider; };
        ShopController.PurchaseCalled += OnPurchaseCalled;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupScreenSize;
    }

    private void OnDisable()
    {
        ShopController.StoreControllerInitialized -= SetupShopProducts;
        ShopController.ExtensionProviderInitialized -= (_extensionProvider) =>  { this._extensionProvider = _extensionProvider; };
        
        ShopController.PurchaseCalled -= OnPurchaseCalled;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupScreenSize;
    }


    private void OnPurchaseCalled(Status status)
    {
        if (status == Status.Success)
            SetButtonInactive();
    }

    private void SetupShopProducts(IStoreController controller = null)
    {
        if (controller == null) return;
        
        _storeController = controller;
        _removeAdProduct = _storeController?.products.WithID(RemoveAdsID);
            
        var hasReceipt = _removeAdProduct.hasReceipt;
        var price = _removeAdProduct?.metadata.localizedPriceString;
        
        _noAdsLabelPrice.style.color = Color.white;
        _noAdsLabelPrice.text = price;

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
        // _noAdsButton.style.backgroundImage = new StyleBackground();
        // _noAdsButton.style.height = new StyleLength(90);
        // _noAdsButton.style.backgroundColor =  new StyleColor(new Color32(0xD3, 0xD1, 0xCE, 0xFF));;
        // _noAdsButton.style.borderBottomLeftRadius = new StyleLength(15);
        // _noAdsButton.style.borderBottomRightRadius = new StyleLength(15);
        // _noAdsButton.style.borderTopLeftRadius = new StyleLength(15);
        // _noAdsButton.style.borderTopRightRadius = new StyleLength(15);
    }
}
