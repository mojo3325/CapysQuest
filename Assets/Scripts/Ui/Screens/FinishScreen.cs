using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FinishScreen : MenuScreen
{
    public static event Action IsShown;

    private Button _okButton;
    private Label _finishLabel;

    private static string _okButtonName = "OkButton";
    private static string _finishLabelName = "FinishText";

    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        _okButton = _root.Q<Button>(_okButtonName);
        _finishLabel = _root.Q<Label>(_finishLabelName);
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _okButton.clicked += _mainMenuUIManager.ShowHomeScreen;
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
    }

    private void SetupFinishLabel(string text)
    {
        _finishLabel.text = text;
    }

    private void OnEnable()
    {
        MenuManagerController.ConnectionIsChecked += _mainMenuUIManager.CheckConnection;
        FinishScreenController.IsReady += SetupFinishLabel;
    }

    private void OnDisable()
    {
        MenuManagerController.ConnectionIsChecked -= _mainMenuUIManager.CheckConnection;
        FinishScreenController.IsReady -= SetupFinishLabel;
    }
}
