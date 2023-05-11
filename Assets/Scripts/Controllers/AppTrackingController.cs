using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif


public class AppTrackingController : MonoBehaviour
{
    #if UNITY_IOS
    public ATTrackingStatusBinding.AuthorizationTrackingStatus _authorizationStatus;
    #endif

    private void Awake() {
    #if UNITY_IOS
        _authorizationStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        if(_authorizationStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
    #endif
    }

    public void RequestTrackingAuthorization()
    {
        #if UNITY_IOS

        _authorizationStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            
        if(_authorizationStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
        #endif
    }
}
