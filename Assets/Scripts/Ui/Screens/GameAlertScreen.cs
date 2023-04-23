using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameAlertScreen : MenuScreen
{
    private string _alert;

    public string Alert
    {
        get => _alert;
        set => _alert = value;
    }
    private Label _alertLabel;

    private static string _alertLabelName = "alert_label";
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;
        _alertLabel = _root.Q<Label>(_alertLabelName);
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        
        if (!string.IsNullOrEmpty(_alert))
        {
            _alertLabel.text = _alert;
        }
    }
}
