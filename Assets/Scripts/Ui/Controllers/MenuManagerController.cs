using Assets.SimpleLocalization;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class MenuManagerController : MonoBehaviour
{
    public static event Action<bool> ConnectionIsChecked;
    public static event Action<VersionFetch> VersionIsChecked;
    private Tools tools = new();
    public struct userAttributes { }
    public struct appAttributes { }

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

    async Task Start()
    {
        SetupGameLanguage();
        CheckConnection();
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

    public void SetupGameLanguage()
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

    async private Task CheckGameVersionAfterConnection(bool isConnected)
    { 
        if (isConnected)
        {

            await InitializeRemoteConfigAsync();

            RemoteConfigService.Instance.FetchCompleted += CheckGameVersionResponse;
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        }
    }

    private void CheckGameVersionResponse(ConfigResponse response)
    {
        if(response.status == ConfigRequestStatus.Success)
        {
            var localVersion = Application.version.Trim();
            var actualVersion = RemoteConfigService.Instance.appConfig.GetString("GameVersion").Trim();

            if (String.Equals(localVersion, actualVersion) == true)
            {
                VersionIsChecked?.Invoke(VersionFetch.Relevant);
            }
            else if(String.Equals(localVersion, actualVersion) == false && actualVersion != "")
            {
                VersionIsChecked?.Invoke(VersionFetch.Old);
            }
            else
            {
                VersionIsChecked?.Invoke(VersionFetch.Failed);
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
