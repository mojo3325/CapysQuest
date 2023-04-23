using System;
using UnityEngine;

public class ConnectionController : MonoBehaviour
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

    private static async void CheckConnection()
    {
        // StartCoroutine(tools.CheckInternetConnection(isConnected =>
        // {
        //     ConnectionIsChecked?.Invoke(isConnected);
        // }));
        var isConnected = await Tools.CheckInternetConnectionAsync();
        ConnectionIsChecked?.Invoke(isConnected); 
    }
}
