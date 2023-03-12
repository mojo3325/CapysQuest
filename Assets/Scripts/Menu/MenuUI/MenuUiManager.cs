using Assets.SimpleLocalization;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

enum LastScreen
{
    MainMenu, GameOver
}
public class MenuUiManager : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private Sprite SoundOn;
    [SerializeField]
    private Sprite SoundOff;

    private Button PlayButton;
    private Button SoundButton;
    private Button SettingsButton;
    private Button SettingsBackButton;
    private VisualElement MainMenu;
    private VisualElement SettingsMenu;
    private VisualElement GameOverMenu;
    private VisualElement MenuControl;
    private LastScreen lastScreen;

    private SoundState soundState = SoundState.On;
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        PlayButton = root.Q<Button>("PlayButton");
        SettingsButton = root.Q<Button>("SettingsButton");
        SettingsBackButton = root.Q<Button>("BackButton");
        SoundButton = root.Q<Button>("SoundButton");

        MainMenu = root.Q<VisualElement>("MainMenu");
        SettingsMenu = root.Q<VisualElement>("SettingsMenu");
        GameOverMenu = root.Q<VisualElement>("GameOverMenu");
        MenuControl = root.Q<VisualElement>("MenuControl");

        if(PlayButton != null)
            PlayButton.clicked += () => OnPlayClick();
        if(SettingsButton != null)
            SettingsButton.clicked += () => OnSettingsClick();
        if(SettingsBackButton != null)
            SettingsBackButton.clicked += () => OnSettingsBackClick();
        if(SoundButton != null)
            SoundButton.clicked += () => SoundTurn();

        EventManager.OnCapyDie.AddListener(OnCapyDie);

    }

    private void OnCapyDie(DieType dieType)
    {
        StartCoroutine(ShowGameOverAferDie());
    }

    IEnumerator ShowGameOverAferDie()
    {
        yield return new WaitForSeconds(1.5f);
        GameOverMenu.style.display = DisplayStyle.Flex;
    }

    private void SoundTurn()
    {
        soundState = (soundState == SoundState.On) ? SoundState.Off : SoundState.On;
        EventManager.OnSoundChangeClick.Invoke(soundState);
        OnSoundButtonClick();
    }

    private void OnSoundButtonClick()
    {
        if(soundState == SoundState.Off)
        {
            SoundButton.style.backgroundImage = new StyleBackground(SoundOff);
        }
        else if(soundState == SoundState.On)
        {
            SoundButton.style.backgroundImage = new StyleBackground(SoundOn);
        }
    }

    private void OnSettingsClick()
    {
        if (SettingsButton != null)
        {
            if (MainMenu.style.display == DisplayStyle.Flex)
                lastScreen = LastScreen.MainMenu;
            MainMenu.style.display = DisplayStyle.None;

            if (GameOverMenu.style.display == DisplayStyle.Flex)
                lastScreen = LastScreen.GameOver;
            GameOverMenu.style.display = DisplayStyle.None;

            MenuControl.style.display = DisplayStyle.None;
            SettingsMenu.style.display = DisplayStyle.Flex;
        }
    }
    private void OnSettingsBackClick()
    {
        if (SettingsBackButton != null)
        {

            SettingsMenu.style.display = DisplayStyle.None;
            MenuControl.style.display = DisplayStyle.Flex;

            if (lastScreen == LastScreen.MainMenu)
            {
                MainMenu.style.display = DisplayStyle.Flex;
            }
            else if (lastScreen == LastScreen.GameOver)
            {
                GameOverMenu.style.display = DisplayStyle.Flex;
            }
        }
    }

    private void OnPlayClick()
    {
        EventManager.OnPlayClick.Invoke();
        MainMenu.style.display = DisplayStyle.None;
    }

    private void Awake()
    {
        LocalizationManager.Read();
        LocalizationManager.Language = "Russian";
        var languagePref = PlayerPrefs.GetString("game_language");

        if(languagePref != null)
        {
            LocalizationManager.Language = languagePref;
        }
        else
        { 

            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    LocalizationManager.Language = "English";
                    PlayerPrefs.SetString("game_language", "English");
                    break;

                case SystemLanguage.Russian:
                    LocalizationManager.Language = "Russian";
                    PlayerPrefs.SetString("game_language", "Russian");
                    break;

                case SystemLanguage.Ukrainian:
                    LocalizationManager.Language = "Ukranian";
                    PlayerPrefs.SetString("game_language", "Ukranian");
                    break;
                default:
                    LocalizationManager.Language = "Russian";
                    PlayerPrefs.SetString("game_language", "Russian");
                    break;
            }
        }
    }
}
