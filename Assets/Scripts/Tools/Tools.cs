using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;
using Ping = System.Net.NetworkInformation.Ping;

public class Tools
{

    public void CheckInternetConnection(Action<bool> state)
    {
        try
        {
            var ping = new Ping();
            byte[] buffer = new byte[32];
            int timeOut = 1000;
            PingOptions options = new PingOptions();
            var reply = ping.Send("8.8.8.8", timeOut, buffer, options);
            state(reply.Status == IPStatus.Success);
        }
        catch
        {
            state(false);
        }

    }
}
