using UnityEngine;
using UnityEngine.UIElements;

public class VersionFailedScreen : MenuScreen
{

    private Button _updateButton;
    private Label _versionFailedLabel;

    private static string _updateButtonName = "UpdateButton";
    private static string _versionFailedLabelName = "VersionLabel";

    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _updateButton = _root.Q<Button>(_updateButtonName);
        _versionFailedLabel = _root.Q<Label>(_versionFailedLabelName);
        SetupSizes();
    }

    private void SetupSizes()
    {
        var devicetype = Tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _versionFailedLabel.style.fontSize = new StyleLength(55);
            _updateButton.style.fontSize = new StyleLength(55);
        }
        else
        {
            _versionFailedLabel.style.fontSize = new StyleLength(45);
            _updateButton.style.fontSize = new StyleLength(45);
        }
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _updateButton.clicked += OpenGamePage;
    }

    private void OpenGamePage()
    {
        
        #if UNITY_IOS
            var gameLink = "https://apps.apple.com/app/id6447211620";
        #elif UNITY_ANDROID
            var gameLink = "https://play.google.com/store/apps/details?id=com.PiderStudio.CapysQuest";
        #endif

        Application.OpenURL(gameLink);
    }
    
}
