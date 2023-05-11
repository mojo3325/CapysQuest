using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class IntAdController : MonoBehaviour
{

    public static event Action AdShown;
    public static event Action OnAdClosed;

    private InterstitialAd interstitialAd;
    
    // private List<string> deviceIds = new List<string>();

    #if UNITY_IOS
        private string _adUnitId = "ca-app-pub-1133247762797902/4763999607";
    #elif UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-1133247762797902/2856817723";
    #else
        string _adUnitId = "unexpected_platform";
    #endif

    private void OnEnable()
    {
        CapyController.CapyDiedThreeTimes += ShowAd;
    }

    private void OnDisable()
    {
        CapyController.CapyDiedThreeTimes -= ShowAd;
    }
    
    private void Start()
    {
        // deviceIds.Add("386B8D0AAB985323CAD395133577A3F5");
        //
        // var requestConfig = new RequestConfiguration
        //         .Builder()
        //         .SetTestDeviceIds(deviceIds)
        //         .build();

        #if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
        #endif
        
        // MobileAds.SetRequestConfiguration(requestConfig);
        MobileAds.Initialize((InitializationStatus initStatus) =>
        { });

        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest
                .Builder()
                .Build();
        
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    return;
                }

                interstitialAd = ad;
            });
    }

    private void ShowAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            AdShown?.Invoke();
            interstitialAd.Show();
            RegisterReloadHandler(interstitialAd);
        }
        else
        {
            LoadInterstitialAd();
        }
    }

    private void RegisterReloadHandler(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            OnAdClosed?.Invoke();
            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            OnAdClosed?.Invoke();
            LoadInterstitialAd();
        };
    }

}
