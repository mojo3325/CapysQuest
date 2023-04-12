using Assets.SimpleLocalization;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static System.String;

public class MenuManagerController : MonoBehaviour
{
    public static event Action<bool> ConnectionIsChecked;
    public static event Action<ConfigResponse> RemoteConfigInitialized;
    public static event Action<VersionFetch> VersionIsChecked;
    private Tools tools = new();

    private struct userAttributes { }

    private struct AppAttributes { }

    async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
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

    private async void Awake()
    {
        SetupGameLanguage();
        CheckConnection();
    }

    private void SetupGameLanguage()
    {
        LocalizationManager.Read();
        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
        }
        else
        {
            if (Application.systemLanguage == SystemLanguage.English)
            {
                saveSelectedLanguage("English");
            }
            else if (Application.systemLanguage == SystemLanguage.Russian)
            {
                saveSelectedLanguage("Russian");
            }
            else if (Application.systemLanguage == SystemLanguage.Ukrainian)
            {
                saveSelectedLanguage("Ukrainian");
            }
            else
            {
                saveSelectedLanguage("English");
            }
        }
    }

    private void saveSelectedLanguage(string language)
    {
        LocalizationManager.Language = language;
        PlayerPrefs.SetString("game_language", language);
    }

    private async Task CheckGameVersionAfterConnection(bool isConnected)
    { 
        if (isConnected)
        {
            await InitializeRemoteConfigAsync();

            RemoteConfigService.Instance.FetchCompleted += CheckGameVersionResponse;
            RemoteConfigService.Instance.FetchCompleted += (response) => RemoteConfigInitialized?.Invoke(response);
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new AppAttributes());
        }
    }

    private void CheckGameVersionResponse(ConfigResponse response)
    {
        if(response.status == ConfigRequestStatus.Success)
        {
            #if UNITY_IOS
                var versionKey = "IOSVersionMin";
            #elif UNITY_ANDROID
                var versionKey = "AndroidVersionMin";
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
        else
        {
            VersionIsChecked?.Invoke(VersionFetch.Failed);
        }
    }

    private void CheckConnection()
    {
        StartCoroutine(tools.CheckInternetConnection(isConnected =>
        {
            ConnectionIsChecked?.Invoke(isConnected);
            _ = CheckGameVersionAfterConnection(isConnected);
        }));
    }
}
