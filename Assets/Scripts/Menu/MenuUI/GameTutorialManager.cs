using Assets.SimpleLocalization;
using UnityEngine;
using UnityEngine.UIElements;

public class GameTutorialManager : MonoBehaviour
{
    private enum TutorialStep
    {
        First, Second
    }

    [SerializeField] private Sprite OkButton;
    private Button nextButton;
    private VisualElement firstTutorial;
    private VisualElement secondTutorial;

    private Label timeInfoLabel;
    private Label jetpackInfoLabel;
    private Label protectionInfoLabel;
    private Label gravityInfoLabel;
    private Label gameItemsLabel;

    private TutorialStep step = TutorialStep.First;

    private void OnEnable()
    {

        LocalizationManager.Read();

        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
        }

        var root = GetComponent<UIDocument>().rootVisualElement;

        nextButton = root.Q<Button>("NextButton");
        firstTutorial = root.Q<VisualElement>("FirstStep");
        secondTutorial = root.Q<VisualElement>("SecondStep");

        protectionInfoLabel = root.Q<Label>("ProtectionInfo");
        timeInfoLabel = root.Q<Label>("TimeInfo");
        jetpackInfoLabel = root.Q<Label>("JetpackInfo");
        gravityInfoLabel = root.Q<Label>("GravityInfo");
        gameItemsLabel = root.Q<Label>("GameItemLabel");

        nextButton.clicked += () => onNextButtonClick();

        SetupItemsInfo();
        EventManager.OnLanguageChange.AddListener(OnLanguageChange);

    }

    private void OnLanguageChange()
    {
        gameItemsLabel.text = LocalizationManager.Localize("game_info");
        protectionInfoLabel.text = LocalizationManager.Localize("Protection");
        timeInfoLabel.text = LocalizationManager.Localize("time");
        jetpackInfoLabel.text = LocalizationManager.Localize("Jetpack");
        gravityInfoLabel.text = LocalizationManager.Localize("Gravity");
    }

    private void SetupItemsInfo()
    {

        gameItemsLabel.text = LocalizationManager.Localize("game_info");
        protectionInfoLabel.text = LocalizationManager.Localize("Protection");
        timeInfoLabel.text = LocalizationManager.Localize("time");
        jetpackInfoLabel.text = LocalizationManager.Localize("Jetpack");
        gravityInfoLabel.text = LocalizationManager.Localize("Gravity");
    }


    private void onNextButtonClick()
    {
        if (step == TutorialStep.First) 
        {
            firstTutorial.style.display = DisplayStyle.None;
            secondTutorial.style.display = DisplayStyle.Flex;
            nextButton.style.backgroundImage = new StyleBackground(OkButton);
            step = TutorialStep.Second;
        }
        else if (step == TutorialStep.Second)
        {
            PlayerPrefs.SetInt("isTutorialAccepted", 1);
            EventManager.OnTutorialAccept.Invoke();
        }
    }
}
