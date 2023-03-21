using System;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    public static event Action<bool> ConnectionIsChecked;
    private Tools tools = new Tools();

    private void OnEnable()
    {
        ConnectionFailedScreen.ConnectionButtonClicked += CheckConnection;
    }

    private void OnDisable()
    {
        ConnectionFailedScreen.ConnectionButtonClicked -= CheckConnection;
    }

    private void CheckConnection()
    {
        tools.CheckInternetConnection(isConnected =>
        {
            ConnectionIsChecked(isConnected);
        });
    }
}
