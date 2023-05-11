using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using Ping = UnityEngine.Ping;

public class ConnectionRepo
{
    public static IEnumerator CheckInternetConnection(Action<bool> state)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            var ping = new Ping("8.8.8.8");
            yield return new WaitForSeconds(1.5f);
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

    public static async Task<bool> CheckInternetConnectionAsync()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create("https://www.google.com");
                request.Timeout = 5000;
                using var response = await request.GetResponseAsync() as HttpWebResponse;
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}