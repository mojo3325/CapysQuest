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
    
    private float DeviceDiagonalSizeInInches()
    {
        var screenWidth = Screen.width / Screen.dpi;
        var screenHeight = Screen.height / Screen.dpi;
        var diagonalInches =Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
        return diagonalInches;
    }
 
    public DeviceType GetDeviceType()
    {
#if UNITY_IOS
        bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        if (deviceIsIpad)
        {
            return DeviceType.Tablet;
        }
 
        bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
        if (deviceIsIphone)
        {
            return DeviceType.Phone;
        }
#endif
         
        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        bool isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);
 
        if (isTablet)
        {
            return DeviceType.Tablet;
        }
        else
        {
            return DeviceType.Phone;
        }
    }
}
