using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialScreen: MenuScreen
{

    public static event Action OnTutorialAccepted;
    private enum TutorialStep
    {
        First, Second
    }

    [SerializeField] private Sprite OkButton;


    private Button _tutorialNextButton;
    private VisualElement _firstTutorial;
    private VisualElement _secondTutorial;

    private Label _timeInfoLabel;
    private Label _jetpackInfoLabel;
    private Label _protectionInfoLabel;
    private Label _gameItemsLabel;

    private static string _tutorialNextButtonName = "NextButton";
    private static string _firstTutorialName = "FirstStep";
    private static string _secondTutorialName = "SecondStep";
    private static string _timeInfoLabelName = "TimeInfo";
    private static string _jetpackInfoLabelName = "JetpackInfo";
    private static string _protectionInfoLabelName = "ProtectionInfo";
    private static string _gameItemsLabelName = "GameItemLabel";

    private TutorialStep step = TutorialStep.First;

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _tutorialNextButton = _root.Q<Button>(_tutorialNextButtonName);
        _firstTutorial = _root.Q<VisualElement>(_firstTutorialName);
        _secondTutorial = _root.Q<VisualElement>(_secondTutorialName);
        _timeInfoLabel = _root.Q<Label>(_timeInfoLabelName);
        _jetpackInfoLabel = _root.Q<Label>(_jetpackInfoLabelName);
        _protectionInfoLabel = _root.Q<Label>(_protectionInfoLabelName);
        _gameItemsLabel = _root.Q<Label>(_gameItemsLabelName);

        SetupLabelsLanguage();

    }

    private void OnEnable()
    {
        SettingsController.LanguageChanged += SetupLabelsLanguage;
    }

    private void OnDisable()
    {
        SettingsController.LanguageChanged -= SetupLabelsLanguage;
    }

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        _tutorialNextButton.clicked += () => OnNextButtonClick();
    }

    private void SetupLabelsLanguage()
    {
        LocalizationManager.Read();

        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
            SetupItemsInfo();
        }
    }

    private void SetupItemsInfo()
    {
        _gameItemsLabel.text = LocalizationManager.Localize("game_info");
        _protectionInfoLabel.text = LocalizationManager.Localize("Protection");
        _timeInfoLabel.text = LocalizationManager.Localize("time");
        _jetpackInfoLabel.text = LocalizationManager.Localize("Jetpack");
    }

    private void OnNextButtonClick()
    {
        if (step == TutorialStep.First) 
        {
            _firstTutorial.style.display = DisplayStyle.None;
            _secondTutorial.style.display = DisplayStyle.Flex;
            _tutorialNextButton.style.backgroundImage = new StyleBackground(OkButton);
            step = TutorialStep.Second;
            SetupItemsInfo();
        }
        else if (step == TutorialStep.Second)
        {
            PlayerPrefs.SetInt("isTutorialAccepted", 1);
            OnTutorialAccepted?.Invoke();
        }
    }
}
