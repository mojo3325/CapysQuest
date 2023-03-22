using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;
using Ping = UnityEngine.Ping;

public class Tools
{

    public IEnumerator CheckInternetConnection(Action<bool> state)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            var ping = new Ping("8.8.8.8");
            yield return new WaitForSeconds(1f);
            try
            {
                if (ping.isDone && ping.time > 0)
                {
                    state(true);
                }
                else
                {
                    state(false);
                }
            }
            catch
            {
                state(false);
            }
        }
        else
        {
            state(false);
        }
    }
}
