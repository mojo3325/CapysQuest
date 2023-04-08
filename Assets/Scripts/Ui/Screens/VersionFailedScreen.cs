using Assets.SimpleLocalization;
using UnityEngine;
using UnityEngine.UIElements;

public class VersionFailedScreen : MenuScreen
{

    private Button _updateButton;
    private Label _versionFailedLabel;

    private static string _updateButtonName = "UpdateButton";
    private static string _versionFailedLabelName = "VersionLabel";
    private Tools _tools = new();
    
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _updateButton = _root.Q<Button>(_updateButtonName);
        _versionFailedLabel = _root.Q<Label>(_versionFailedLabelName);
        LocalizationManager.Read();

        SetupSizes();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        SetupScreen();
    }

    private void SetupScreen()
    {

        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
            _updateButton.style.color = Color.white;
            _versionFailedLabel.text = LocalizationManager.Localize("version_error");
            _updateButton.text = LocalizationManager.Localize("version_update");
        }
        else
        {
            _updateButton.style.color = Color.white;
            _versionFailedLabel.text = LocalizationManager.Localize("Looks like an old version of the game is installed (");
            _updateButton.text = LocalizationManager.Localize("Update");
        }
    }
    

    private void SetupSizes()
    {
        var devicetype = _tools.GetDeviceType();

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
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                Application.OpenURL("https://play.google.com/store/games");
                break;
            case RuntimePlatform.IPhonePlayer:
                Application.OpenURL("https://www.apple.com/app-store/");
                break;
            default:
                Application.OpenURL("https://play.google.com/store/games");
                break;
        }
    }
}
