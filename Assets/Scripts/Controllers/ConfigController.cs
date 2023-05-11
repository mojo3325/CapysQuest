using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.VisualScripting;

public class ConfigController : MonoBehaviour
{
    public static event Action<ConfigResponse> RemoteConfigInitialized;
    public static event Action<VersionFetch> VersionIsChecked;
    public static event Action GameOnTechnicalBreak;
    private struct userAttributes { }
    private struct AppAttributes { }

    private bool _remoteConfigInitialized;

    private void OnEnable()
    {
        ConnectionController.ConnectionIsChecked += FetchRemoteConfig;
    }

    private void OnDisable()
    {
        ConnectionController.ConnectionIsChecked -= FetchRemoteConfig;
    }

    private async void FetchRemoteConfig(bool isConnected)
    {
        await FetchRemoteConfigAsync(isConnected);
    }
    
    private static async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();
    }

    private async Task FetchRemoteConfigAsync(bool isConnected)
    { 
        if (isConnected)
        {
            if (_remoteConfigInitialized != true)
            {
                await InitializeRemoteConfigAsync();
                RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetchCompleted;
                RemoteConfigService.Instance.FetchCompleted += (response) => RemoteConfigInitialized?.Invoke(response);
                await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new AppAttributes());
            }
            else
            {
                CheckGameVersion();
                CheckGameStatus();
            }
        }
    }

    private void OnRemoteConfigFetchCompleted(ConfigResponse response)
    {
        if(response.status == ConfigRequestStatus.Success)
        {
            _remoteConfigInitialized = true;
            CheckGameVersion();
            CheckGameStatus();
        }
        else
        {
            VersionIsChecked?.Invoke(VersionFetch.Failed);
        }
    }

    private static void CheckGameVersion()
    {
        #if UNITY_IOS
                const string versionKey = "IOSVersionMin";
        #elif UNITY_ANDROID
                const string versionKey = "AndroidVersionMin";
        #endif
            
        var localVersion = new Version(Application.version);
        var requiredVersion = new Version(RemoteConfigService.Instance.appConfig.GetString(versionKey));

        if (localVersion < requiredVersion)
        {
            VersionIsChecked?.Invoke(VersionFetch.Old);
        }

        if (localVersion >= requiredVersion)
        {
            VersionIsChecked?.Invoke(VersionFetch.Relevant);
        }
    }

    private static void CheckGameStatus()
    {
        const string technicalBreakKey = "IsTechnicalBreak";
        var isGameOnTechnicalBreak = RemoteConfigService.Instance.appConfig.GetBool(technicalBreakKey);
    
        if (isGameOnTechnicalBreak)
        {
            GameOnTechnicalBreak?.Invoke();
        }
    }
}
