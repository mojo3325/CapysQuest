using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopController : MonoBehaviour, IStoreListener
{
    public static event Action PurchaseIsSuccessful;
    public static event Action PurchaseIsFailed;
    public static event Action<IStoreController> StoreControllerInitialized;
    
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;

    public IStoreController StoreController
    {
        get => _storeController;
        private set => _storeController = value;
    }
    
    private async void Awake()
    {
        var options = new InitializationOptions()

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    .SetEnvironmentName("test");
        #else
                    .SetEnvironmentName("production");
        #endif

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
                    PurchaseIsFailed?.Invoke();
                }
            }
            else
            {
                PurchaseIsFailed?.Invoke();
            }
        }
        catch (Exception e)
        {
            PurchaseIsFailed?.Invoke();
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
        PlayerPrefs.SetInt("RemoveAds", 1);
        Debug.Log("PurchaseSuccess");
        PurchaseIsSuccessful?.Invoke();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        PurchaseIsFailed?.Invoke();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _extensionProvider = extensions;

        StoreControllerInitialized?.Invoke(_storeController);
    }
}