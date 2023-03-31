using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class IntAdController : MonoBehaviour
{

    public static event Action OnAdSuccessClosed;

    private InterstitialAd interstitialAd;
    

    #if UNITY_IPHONE
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


    private async void Start()
    {

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

        var adRequest = new AdRequest.Builder()
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
            OnAdSuccessClosed?.Invoke();

            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadInterstitialAd();
        };
    }

}
