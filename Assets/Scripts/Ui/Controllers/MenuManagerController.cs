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
    public static event Action<bool> VersionIsChecked;
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

    async Task Start()
    {
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += CheckGameVersion;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    private void OnEnable()
    {
        MainMenuUIManager.IsEnabled += SetupGameLanguage;
        MainMenuUIManager.IsEnabled += CheckConnection;
        MainMenuUIManager.IsFocused += CheckConnection;
        MainMenuUIManager.IsPaused += CheckConnection;
        GameOverScreen.IsShown += CheckConnection;
        SettingsScreen.IsShown += CheckConnection;
        GameScreen.IsShown += CheckConnection;
        FinishScreen.IsShown += CheckConnection;
    }

    private void OnDisable()
    {
        MainMenuUIManager.IsEnabled -= SetupGameLanguage;
        MainMenuUIManager.IsEnabled -= CheckConnection;
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

    private void CheckGameVersion(ConfigResponse response)
    {
        var localVersion = Application.version.Trim();
        var actualVersion = RemoteConfigService.Instance.appConfig.GetString("GameVersion").Trim();

        if (String.Equals(localVersion, actualVersion) == true)
        {
            VersionIsChecked?.Invoke(true);
        }
        else
        {
            VersionIsChecked?.Invoke(false);
        }
    }

    private void CheckConnection()
    {
        StartCoroutine(tools.CheckInternetConnection(isConnected =>
        {
            ConnectionIsChecked?.Invoke(isConnected);
        }));
    }
}
