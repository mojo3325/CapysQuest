using System;
using System.Collections;
#if UNITY_ANDROID
using System.Collections.Generic;
using Google.Play.Review;
#endif
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class GameRateController : MonoBehaviour
{
    #if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    #endif
    
    private void Start()
    {
        CallGameRate();
    }

    private void CallGameRate()
    {
        var launchCount = PlayerPrefs.GetInt("LaunchCount", 0);
        var appRated = PlayerPrefs.GetInt("AppRated", 0);
        
        launchCount += 1;
        PlayerPrefs.SetInt("LaunchCount", launchCount);

        #if UNITY_ANDROID

        if (launchCount is 3 or 5 && appRated == 0)
        {
                StartCoroutine(GameRateCall());   
        }
        
        #endif

        
        #if UNITY_IOS
        
        if (launchCount is 3 or 5)
        {
             Device.RequestStoreReview();
        }
                 
        #endif
        
    }

    private IEnumerator GameRateCall()
    {
#if UNITY_ANDROID
        
        _reviewManager = new ReviewManager();

        if (_reviewManager == null)
            _reviewManager = new ReviewManager();
        
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        
        yield return requestFlowOperation;
        
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }

        PlayerPrefs.SetInt("AppRated", 1);
#endif
        yield return null;
    } 
}
