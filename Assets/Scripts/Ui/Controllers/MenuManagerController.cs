using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.MiniJSON;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManagerController : MonoBehaviour
{
    public static event Action<bool> ConnectionIsChecked;
    public static event Action<ConfigResponse> RemoteConfigInitialized;
    public static event Action<VersionFetch> VersionIsChecked;
    public static event Action GameOnTechnicalBreak;
    
    public static event Action<Dictionary<string, object>> GameStringsInitialized;

    private struct userAttributes { }
    private struct AppAttributes { }

    private bool _remoteConfigInitialized;

    private static async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();

        // if (!AuthenticationService.Instance.IsSignedIn)
        // {
        //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        // }
    }

    private void OnEnable()
    {
        MainMenuUIManager.IsFocused += CheckConnection;
        MainMenuUIManager.IsPaused += CheckConnection;
        GameOverScreen.IsShown += CheckConnection;
        SettingsScreen.IsShown += CheckConnection;
        GameScreen.IsShown += CheckConnection;
        FinishScreen.IsShown += CheckConnection;
    }
    
    private void OnDisable()
    {
        MainMenuUIManager.IsFocused -= CheckConnection;
        MainMenuUIManager.IsPaused -= CheckConnection;
        GameOverScreen.IsShown -= CheckConnection;
        SettingsScreen.IsShown -= CheckConnection;
        GameScreen.IsShown -= CheckConnection;
        FinishScreen.IsShown -= CheckConnection;
    }

    private void Awake()
    {
        CheckConnection();
    }

    private void Start()
    {
        var myStringsFile = Resources.Load("GameStrings");
        var gameStrings = Json.Deserialize(myStringsFile.ToString()) as Dictionary<string, object>;
        
        if (gameStrings != null && gameStrings.Count != 0)
        {
            GameStringsInitialized?.Invoke(gameStrings);
        }
    }

    private async Task FetchRemoteConfigAsync(bool isConnected)
    { 
        if (isConnected)
        {
            if (_remoteConfigInitialized != true)
            {
                await InitializeRemoteConfigAsync();
                await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new AppAttributes());
                RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetchCompleted;
                RemoteConfigService.Instance.FetchCompleted += (response) => RemoteConfigInitialized?.Invoke(response);
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
    
    private async void CheckConnection()
    {
        // StartCoroutine(tools.CheckInternetConnection( isConnected =>
        // {
        //     ConnectionIsChecked?.Invoke(isConnected); 
        //     CheckGameVersionAfterConnection(isConnected);
        // }));
        
        var isConnected = await Tools.CheckInternetConnectionAsync();
        ConnectionIsChecked?.Invoke(isConnected); 
        await FetchRemoteConfigAsync(isConnected);
    }
}



