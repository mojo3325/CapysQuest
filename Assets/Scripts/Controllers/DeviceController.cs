using System;
using UnityEngine;

public class DeviceController : MonoBehaviour
{
    [SerializeField] private DeviceType _deviceType;
    public DeviceType DeviceType => _deviceType;

    public event Action DeviceTypeFetched;
    
    public void SyncDeviceType()
    {
        GetDeviceType();
    }

    private void Start()
    {
        GetDeviceType();
    }

    public static float DeviceDiagonalSizeInInches()
    {
        var screenWidth = Screen.width / Screen.dpi;
        var screenHeight = Screen.height / Screen.dpi;
        var diagonalInches =Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
        return diagonalInches;
    }
 
    private void GetDeviceType()
    {
        #if UNITY_IOS
            bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
            {
                _deviceType = DeviceType.Tablet;
                DeviceTypeFetched?.Invoke();
            }
     
            bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
            if (deviceIsIphone)
            {
                _deviceType = DeviceType.Phone;
                DeviceTypeFetched?.Invoke();
            }
        #endif
         
        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        bool isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);
 
        if (isTablet)
        {
            _deviceType = DeviceType.Tablet;
            DeviceTypeFetched?.Invoke();
        }
        else
        {
            _deviceType = DeviceType.Phone;
            DeviceTypeFetched?.Invoke();
        }
    }
}


