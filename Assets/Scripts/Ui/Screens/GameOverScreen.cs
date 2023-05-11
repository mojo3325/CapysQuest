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
    
    private VisualElement _focusCircle1;
    private VisualElement _focusCircle2;
    private VisualElement _focusCircle3;
    private VisualElement _focusCircle4;
    private VisualElement _focusCircleFinish;
    
    private VisualElement _zoneContainer;
    private VisualElement _zoneContainer1;
    private VisualElement _zoneContainer2;
    private VisualElement _zoneContainer3;
    private VisualElement _zoneContainer4;
    private VisualElement _finishZoneContainer;

    private Label _gameOverLabel;

    private bool _zone1Achieved;
    private bool _zone2Achieved;
    private bool _zone3Achieved;
    private bool _zone4Achieved;
    private bool _finishAchieved;

    private static string _gameOverLabelName = "GameOverLabel";
    private static string _zoneContainerName = "ZoneContainer";
    
    private static string _zoneContainer1Name = "zone1_container";
    private static string _zoneContainer2Name = "zone2_container";
    private static string _zoneContainer3Name = "zone3_container";
    private static string _zoneContainer4Name = "zone4_container";
    private static string _finishZoneContainerName = "zoneFinish_container";
    
    private DeviceType _deviceType;
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = true;
        _gameOverLabel = _root.Q<Label>(_gameOverLabelName);
        
        _zoneContainer1 = _root.Q<VisualElement>(_zoneContainer1Name);
        _zoneContainer2 = _root.Q<VisualElement>(_zoneContainer2Name);
        _zoneContainer3 = _root.Q<VisualElement>(_zoneContainer3Name);
        _zoneContainer4 = _root.Q<VisualElement>(_zoneContainer4Name);
        _finishZoneContainer = _root.Q<VisualElement>(_finishZoneContainerName);

        _zone1 = _zoneContainer1.Query<VisualElement>("ZoneFlag");
        _zone2 = _zoneContainer2.Query<VisualElement>("ZoneFlag");
        _zone3 = _zoneContainer3.Query<VisualElement>("ZoneFlag");
        _zone4 = _zoneContainer4.Query<VisualElement>("ZoneFlag");
        _finishZone = _finishZoneContainer.Query<VisualElement>("ZoneFlag");
        
        _focusCircle1 = _zoneContainer1.Query<VisualElement>("FocusCircle");
        _focusCircle2 = _zoneContainer2.Query<VisualElement>("FocusCircle");
        _focusCircle3 = _zoneContainer3.Query<VisualElement>("FocusCircle");
        _focusCircle4 = _zoneContainer4.Query<VisualElement>("FocusCircle");
        _focusCircleFinish = _finishZoneContainer.Query<VisualElement>("FocusCircle");
        
        _zoneContainer = _root.Q<VisualElement>(_zoneContainerName);
    }

    private void OnEnable()
    {
        ZoneController.OnZoneAchieved += SetZoneAchieved;
        MenuBar.PlayButtonClicked += ResetUserProgress;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched += SetupSizes;
    }

    private void OnDisable()
    {
        ZoneController.OnZoneAchieved -= SetZoneAchieved;
        MenuBar.PlayButtonClicked -= ResetUserProgress;
        _mainMenuUIManager.DeviceController.DeviceTypeFetched -= SetupSizes;
    }

    private void SetupSizes()
    {
        _deviceType = _mainMenuUIManager.DeviceController.DeviceType;

        if (string.IsNullOrEmpty(_deviceType.ToString()))
        {
            _mainMenuUIManager.DeviceController.SyncDeviceType();
            _deviceType = _mainMenuUIManager.DeviceController.DeviceType;
        }
        
        if (_deviceType == DeviceType.Phone)
        {
            _gameOverLabel.style.fontSize = new StyleLength(60);
            _zoneContainer.style.width = Length.Percent(80);
            _zoneContainer.style.height = Length.Percent(50);
        }
        if(_deviceType == DeviceType.Tablet)
        {
            _gameOverLabel.style.fontSize = new StyleLength(40);
            _zoneContainer.style.width = Length.Percent(90);
            _zoneContainer.style.height = Length.Percent(40);
        }
    }
    
    
    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
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
        if (_zone1Achieved)
        {
            _zone1.style.opacity = 1f;
            _focusCircle1.style.display = DisplayStyle.Flex;
        }
        else
        {
            _zone1.style.opacity = 0.4f;
            _focusCircle1.style.display = DisplayStyle.None;
        }
        if (_zone2Achieved)
        {
            _zone2.style.opacity = 1f;
            _focusCircle2.style.display = DisplayStyle.Flex;
        }
        else
        {
            _zone2.style.opacity = 0.4f;
            _focusCircle2.style.display = DisplayStyle.None;
        }
        if (_zone3Achieved)
        {
            _zone3.style.opacity = 1f;
            _focusCircle3.style.display = DisplayStyle.Flex;
        }
        else
        {
            _zone3.style.opacity = 0.4f;
            _focusCircle3.style.display = DisplayStyle.None;
        }
        if (_zone4Achieved)
        {
            _zone4.style.opacity = 1f;
            _focusCircle4.style.display = DisplayStyle.Flex;
        }
        else
        {
            _zone4.style.opacity = 0.4f;
            _focusCircle4.style.display = DisplayStyle.None;
        }
        if (_finishAchieved)
        {
            _finishZone.style.opacity = 1f;
            _focusCircleFinish.style.display = DisplayStyle.Flex;
        }
        else
        {
            _finishZone.style.opacity = 0.4f;
            _focusCircleFinish.style.display = DisplayStyle.None;
        }
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