using Assets.SimpleLocalization;
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
        _updateButton = _root.Q<Button>(_updateButtonName);
        _versionFailedLabel = _root.Q<Label>(_versionFailedLabelName);
        LocalizationManager.Read();
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
