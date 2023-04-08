using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopController : MonoBehaviour, IStoreListener
{
    public static event Action<TransactionStatus> PurchaseCalled;
    public static event Action<IStoreController> StoreControllerInitialized;
    public static event Action<IExtensionProvider> ExtensionProviderInitialized;
    
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;

    private async void Awake()
    {
        var options = new InitializationOptions()
                .SetEnvironmentName("production");

        await UnityServices.InitializeAsync(options);
        var operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }

    private void OnEnable()
    {
        ShopScreen.NoAdsButtonClicked += HandleNoAdPurchase;
    }
    
    private void OnDisable()
    {
        ShopScreen.NoAdsButtonClicked -= HandleNoAdPurchase;
    }
    
    private bool IsInitialized()
    {
        return _storeController != null && _extensionProvider != null;
    }
    
    private void HandleNoAdPurchase(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                var product = _storeController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    PurchaseCalled?.Invoke(TransactionStatus.Failed);
                }
            }
            else
            {
                PurchaseCalled?.Invoke(TransactionStatus.Failed);
            }
        }
        catch (Exception e)
        {
            PurchaseCalled?.Invoke(TransactionStatus.Failed);
            
        }
    }
    
    private void HandleIAPCatalogLoaded(AsyncOperation operation)
    {
        var request = operation as ResourceRequest;
        var catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        
#if UNITY_ANDROID
        var builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
#elif UNITY_IOS 
        var builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else 
        var builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.fake)
        );
#endif

        foreach (var product in catalog.allProducts)
        {
            builder.AddProduct(product.id, product.type);
        }
        
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("On IAP InitializeFailed BECAUSE: => "  + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string? message)
    {
        Debug.Log("On IAP InitializeFailed BECAUSE: => "  + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        
    #if UNITY_IOS
            var RemoveAdsID = "com.PiderStudio.CapysQuest.RemoveAdvertisement";
    #elif UNITY_ANDROID 
        var RemoveAdsID = "com.piderstudio.capysquest.removeadvertisement";  
    #endif

        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, RemoveAdsID, StringComparison.Ordinal))
        {
            PlayerPrefs.SetInt("RemoveAds", 1);
            PurchaseCalled?.Invoke(TransactionStatus.Success);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        PurchaseCalled?.Invoke(TransactionStatus.Failed);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _extensionProvider = extensions;

        StoreControllerInitialized?.Invoke(_storeController);
    }
}