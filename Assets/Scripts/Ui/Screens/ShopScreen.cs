using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

public class ShopScreen : MenuScreen
{
    public static event Action<string> NoAdsButtonClicked;
    
    private Label _noAdsLabel;
    // private Label _2shopItemLabel;
    // private Label _3shopItemLabel;
    
    private Button _backButton;
    private Button _noAdsButton;
    private Button _restorePurchasesButton;
    
    private static string _backButtonName = "ShopBackButton";
    private static string _noAdsButtonName = "NoAdsButton";
    private static string _noAdsLabelName = "NoAdsLabel";
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

    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        
        _backButton = _root.Q<Button>(_backButtonName);
        _noAdsButton = _root.Q<Button>(_noAdsButtonName);
        _noAdsLabel = _root.Q<Label>(_noAdsLabelName);
        _restorePurchasesButton = _root.Q<Button>(_restorePurchasesButtonName);
        // _2shopItemLabel = _root.Q<Label>(_2shopItemLabelName);
        // _3shopItemLabel = _root.Q<Label>(_3shopItemLabelName);
        SetupSizes();
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
    
    private void SetupSizes()
    {
        var devicetype = Tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _noAdsLabel.style.fontSize = new StyleLength(50);
            // _2shopItemLabel.style.fontSize = new StyleLength(50);
            // _3shopItemLabel.style.fontSize = new StyleLength(50);
            _noAdsButton.style.fontSize = new StyleLength(45);
            _restorePurchasesButton.style.fontSize = new StyleLength(30);
        }
        else
        {
            _noAdsLabel.style.fontSize = new StyleLength(30);
            // _2shopItemLabel.style.fontSize = new StyleLength(30);
            // _3shopItemLabel.style.fontSize = new StyleLength(30);
            _noAdsButton.style.fontSize = new StyleLength(30);
            _restorePurchasesButton.style.fontSize = new StyleLength(25);
        }
    }

    private void OnEnable()
    {
        
        ShopController.StoreControllerInitialized += (c) => SetupShopProducts(controller: c);
        ShopController.ExtensionProviderInitialized += (_extensionProvider) =>  { this._extensionProvider = _extensionProvider; };
        
        ShopController.PurchaseCalled += OnPurchaseCalled;
    }

    private void OnDisable()
    {
        ShopController.StoreControllerInitialized -= (c) => SetupShopProducts(controller: c);
        ShopController.ExtensionProviderInitialized -= (_extensionProvider) =>  { this._extensionProvider = _extensionProvider; };
        
        ShopController.PurchaseCalled -= OnPurchaseCalled;
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
