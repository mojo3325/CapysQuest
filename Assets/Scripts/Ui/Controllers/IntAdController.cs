using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class IntAdController : MonoBehaviour
{

    public static event Action OnAdSuccessClosed;

    private InterstitialAd interstitialAd;

    #if UNITY_ANDROID
              private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
    #elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
    #else
              private string _adUnitId = "unused";
    #endif

    private void OnEnable()
    {
        //IntAdScreen.IsShown += ShowAd;

        CapyController.CapyDiedThreeTimes += ShowAd;
    }

    private void OnDisable()
    {
        CapyController.CapyDiedThreeTimes += ShowAd;

        //IntAdScreen.IsShown -= ShowAd;
    }


    private void Start()
    {

        MobileAds.Initialize((InitializationStatus initStatus) =>
        { });

        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
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

    public void ShowAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            RegisterReloadHandler(interstitialAd);
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
