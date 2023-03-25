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
        { 
            Debug.Log("Ad init status => " + initStatus);
        });

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
                Debug.Log("On interstitialAd load");
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                interstitialAd = ad;
            });
    }

    public void ShowAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("OnShowInt AD called after 3 dies");
            interstitialAd.Show();
            RegisterReloadHandler(interstitialAd);
        }
    }

    private void RegisterReloadHandler(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("OnAdFullScreenContentClosed Succes Called");

            OnAdSuccessClosed?.Invoke();

            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            LoadInterstitialAd();
        };
    }

}
