using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionScreenController : MonoBehaviour
{
    public static event Action<bool> ConnectionIsChecked;

    private void OnEnable()
    {
        ConnectionFailedScreen.ConnectionButtonClicked += CheckConnection;
    }

    private void OnDisable()
    {
        ConnectionFailedScreen.ConnectionButtonClicked -= CheckConnection;
    }
    
    private async void CheckConnection()
    {
        var isConnected = await ConnectionRepo.CheckInternetConnectionAsync();
        ConnectionIsChecked?.Invoke(isConnected);
    }
}
