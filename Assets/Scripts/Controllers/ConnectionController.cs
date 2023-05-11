using System;
using UnityEngine;
public class ConnectionController : MonoBehaviour
{
    public static event Action<bool> ConnectionIsChecked;
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
        ConnectionFailedScreen.ConnectionButtonClicked -= CheckConnection;
    }
    
    private async void Awake()
    {
        var isConnected = await ConnectionRepo.CheckInternetConnectionAsync();
        ConnectionIsChecked?.Invoke(isConnected);
    }
    
    private async void CheckConnection()
    {
        var isConnected = await ConnectionRepo.CheckInternetConnectionAsync();
        ConnectionIsChecked?.Invoke(isConnected);
    }
}
