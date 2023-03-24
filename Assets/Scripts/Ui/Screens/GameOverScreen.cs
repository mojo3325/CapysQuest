using Assets.SimpleLocalization;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : MenuScreen
{
    public static event Action IsShown;


    private VisualElement _zone1;
    private VisualElement _zone2;
    private VisualElement _zone3;
    private VisualElement _zone4;
    private VisualElement _finishZone;

    private Label _gameOverLabel;

    private bool _zone1Achieved;
    private bool _zone2Achieved;
    private bool _zone3Achieved;
    private bool _zone4Achieved;
    private bool _finishAchieved;

    private static string _zone1Name = "Zone1";
    private static string _zone2Name = "Zone2";
    private static string _zone3Name = "Zone3";
    private static string _zone4Name = "Zone4";
    private static string _finishZoneName = "FinishZone";
    private static string _gameOverLabelName = "GameOverLabel";

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = true;
        _gameOverLabel = _root.Q<Label>(_gameOverLabelName);
        _zone1 = _root.Q<VisualElement>(_zone1Name);
        _zone2 = _root.Q<VisualElement>(_zone2Name);
        _zone3 = _root.Q<VisualElement>(_zone3Name);
        _zone4 = _root.Q<VisualElement>(_zone4Name);
        _finishZone = _root.Q<VisualElement>(_finishZoneName);

    }

    private void OnEnable()
    {
        CapyCharacter.OnZoneAchieved += SetZoneAchieved;
        MenuBar.PlayButtonClicked += ResetUserProgress;
        SettingsController.LanguageChanged += OnLanguageChange;
        MenuManagerController.ConnectionIsChecked += _mainMenuUIManager.CheckConnection;
    }

    private void OnDisable()
    {
        CapyCharacter.OnZoneAchieved -= SetZoneAchieved;
        MenuBar.PlayButtonClicked -= ResetUserProgress;
        SettingsController.LanguageChanged -= OnLanguageChange;
        MenuManagerController.ConnectionIsChecked -= _mainMenuUIManager.CheckConnection;
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
        LocalizationManager.Read();
        SetupGameOverScreen();
    }

    private void ResetUserProgress()
    {
        _zone1Achieved = false;
        _zone2Achieved = false;
        _zone3Achieved = false;
        _zone4Achieved = false;
        _finishAchieved = false;
    }

    private void SetupGameOverScreen()
    {
        var text = LocalizationManager.Localize("afterDie_label");
        _gameOverLabel.text = text;

        if (_zone1Achieved)
            _zone1.style.opacity = 1f;
        if (_zone2Achieved)
            _zone2.style.opacity = 1f;
        if (_zone3Achieved)
            _zone3.style.opacity = 1f;
        if (_zone4Achieved)
            _zone4.style.opacity = 1f;
        if (_finishAchieved)
            _finishZone.style.opacity = 1f;
    }

    private void OnLanguageChange()
    {
        var text = LocalizationManager.Localize("afterDie_label");
        _gameOverLabel.text = text;
    }

    private void SetZoneAchieved(ZoneType zone)
    {
        switch(zone)
        {
            case ZoneType.zone_1:
                _zone1Achieved = true;
                break;
            case ZoneType.zone_2:
                _zone2Achieved = true;
                break;
            case ZoneType.zone_3:
                _zone3Achieved = true;
                break;
            case ZoneType.zone_4:
                _zone4Achieved = true;
                break;
            case ZoneType.zone_finish:
                _finishAchieved = true;
                break;
        }
    }
}
