using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopController : MonoBehaviour, IStoreListener
{
    public static event Action PurchaseIsCompleted;
    
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;
    
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


    private void HandleNoAdPurchase()
    {
        _storeController.InitiatePurchase("com.piderstudio.capysquest.removeads");
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
        Debug.Log("Purchase Complete");
        PurchaseIsCompleted?.Invoke();
        PlayerPrefs.SetInt("RemoveAds", 1);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        PurchaseIsCompleted?.Invoke();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _extensionProvider = extensions;
    }
}
