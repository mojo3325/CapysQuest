using Assets.SimpleLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameScreen : MenuScreen
{
    public static event Action RightButtonClicked;
    public static event Action LeftButtonClicked;
    public static event Action IsShown;









    //HACK


    public static event Action Zone1Clicked;
    public static event Action Zone2Clicked;
    public static event Action Zone3Clicked;
    public static event Action Zone4Clicked;
    public static event Action CloseClicked;
    public static event Action FinishClicked;

    private VisualElement _hackContainer;
    private VisualElement _timeContainer;

    Button Zone1Button;
    Button Zone2Button;
    Button Zone3Button;
    Button Zone4Button;
    Button FinishButton;
    Button CloseButton;


    private VisualElement _fish1;
    private VisualElement _fish2;
    private VisualElement _fish3;
    private VisualElement _fish4;
    private VisualElement _fish5;




    //HACK









    private Button _rightTapButton;
    private Button _leftTapButton;

    private Label _gameLabel;

    private static string _fish1Name = "Fish1";
    private static string _fish2Name = "Fish2";
    private static string _fish3Name = "Fish3";
    private static string _fish4Name = "Fish4";
    private static string _fish5Name = "Fish5";
    private static string _rightTapButtonName = "RightTapButton";
    private static string _leftTapButtonName = "LeftTapButton";
    private static string _gameLabelName = "GameText";


    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _fish1 = _root.Q<VisualElement>(_fish1Name);
        _fish2 = _root.Q<VisualElement>(_fish2Name);
        _fish3 = _root.Q<VisualElement>(_fish3Name);
        _fish4 = _root.Q<VisualElement>(_fish4Name);
        _fish5 = _root.Q<VisualElement>(_fish5Name);
        _rightTapButton = _root.Q<Button>(_rightTapButtonName);
        _leftTapButton = _root.Q<Button>(_leftTapButtonName);
        _gameLabel = _root.Q<Label>(_gameLabelName);






        //HACK


        Zone1Button = _root.Q<Button>("Zone1Button");
        Zone2Button = _root.Q<Button>("Zone2Button");
        Zone3Button = _root.Q<Button>("Zone3Button");
        Zone4Button = _root.Q<Button>("Zone4Button");
        FinishButton = _root.Q<Button>("FinishButton");
        CloseButton = _root.Q<Button>("CloseButton");
        _hackContainer = _root.Q<VisualElement>("HackContainer");
        _timeContainer = _root.Q<VisualElement>("TimeContainer");

        //HACK





        LocalizationManager.Read();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        IsShown?.Invoke();
    }

    private void OnEnable()
    {
        CapyController.OnTimeChanged += SetupTime;
        CapyController.OnTimeLost += ShowTimeLostText;
        CapyCharacter.TimeClaimed += ShowTimeReachedText;
        ZoneController.OnZoneAchieved += ShowZoneReachedText;
    }

    private void OnDisable()
    {
        CapyController.OnTimeChanged -= SetupTime;
        CapyController.OnTimeLost -= ShowTimeLostText;
        CapyCharacter.TimeClaimed -= ShowTimeReachedText;
        ZoneController.OnZoneAchieved -= ShowZoneReachedText;
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _rightTapButton.clicked += () => RightButtonClicked?.Invoke();
        _leftTapButton.clicked += () => LeftButtonClicked?.Invoke();



        //HACK


        _timeContainer.RegisterCallback < MouseDownEvent>(ShowHackMenu, TrickleDown.TrickleDown);
        CloseButton.clicked += () => HideHackMenu();
        Zone1Button.clicked += () => Zone1Clicked?.Invoke();
        Zone2Button.clicked += () => Zone2Clicked?.Invoke();
        Zone3Button.clicked += () => Zone3Clicked?.Invoke();
        Zone4Button.clicked += () => Zone4Clicked?.Invoke();
        FinishButton.clicked += () => FinishClicked?.Invoke();


        //HACK


    }




    //HACK


    void ShowHackMenu(MouseDownEvent evt)
    {
        _hackContainer.style.display = DisplayStyle.Flex;
        _timeContainer.style.display = DisplayStyle.None;
    }

    void HideHackMenu()
    {
        _hackContainer.style.display = DisplayStyle.None;
        _timeContainer.style.display = DisplayStyle.Flex;
    }

    //HACK








    private void ShowTimeLostText()
    {
        var text = LocalizationManager.Localize("time_lost");
       StartCoroutine(ShowText(text));
    }

    private void ShowZoneReachedText(ZoneType zoneType)
    {
        var text = LocalizationManager.Localize(zoneType.ToString());
       StartCoroutine(ShowText(text));
    }

    private void ShowTimeReachedText()
    {
        var text = LocalizationManager.Localize("time_booster");
        StartCoroutine(ShowText(text));
    }

    private IEnumerator ShowText(string text = "") 
    {
        _gameLabel.style.display = DisplayStyle.Flex;
        _gameLabel.style.opacity = 1f;
        _gameLabel.text = text;
        yield return new WaitForSeconds(1.5f);
        _gameLabel.style.display = DisplayStyle.None;
        _gameLabel.style.opacity = 0f;
    }


    private void SetupTime(float time)
    {
        _fish1.style.display = time < 1 ? DisplayStyle.None : DisplayStyle.Flex;
        _fish2.style.display = time < 2 ? DisplayStyle.None : DisplayStyle.Flex;
        _fish3.style.display = time < 3 ? DisplayStyle.None : DisplayStyle.Flex;
        _fish4.style.display = time < 4 ? DisplayStyle.None : DisplayStyle.Flex;
        _fish5.style.display = time < 5 ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
