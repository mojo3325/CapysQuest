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
    GameController gameController;

    private Button PlayButton;
    private Button MuteButton;
    private Button UnmuteButton;
    private Button SettingsButton;
    private Button SettingsBackButton;
    private VisualElement MainMenu;
    private VisualElement SettingsMenu;
    private VisualElement GameOverMenu;
    private VisualElement MenuControl;
    private LastScreen lastScreen;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        PlayButton = root.Q<Button>("PlayButton");
        SettingsButton = root.Q<Button>("SettingsButton");
        SettingsBackButton = root.Q<Button>("BackButton");
        MuteButton = root.Q<Button>("MuteButton");
        UnmuteButton = root.Q<Button>("UnmuteButton");

        MainMenu = root.Q<VisualElement>("MainMenu");
        SettingsMenu = root.Q<VisualElement>("SettingsMenu");
        GameOverMenu = root.Q<VisualElement>("GameOverMenu");
        MenuControl = root.Q<VisualElement>("MenuControl");

        PlayButton.clicked += () => OnPlayClick();
        SettingsButton.clicked += () => OnSettingsClick();
        SettingsBackButton.clicked += () => OnSettingsBackClick();
        MuteButton.clicked += () => gameController.SoundMute();
        UnmuteButton.clicked += () => gameController.SoundUnmute();

        EventManager.OnSoundMuteClick.AddListener(OnSoundMute);
        EventManager.OnSoundUnmuteClick.AddListener(OnSoundUnmute);
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

    private void OnSoundMute()
    {
        if (MuteButton != null && UnmuteButton != null)
        {
            MuteButton.style.display = DisplayStyle.None;
            UnmuteButton.style.display = DisplayStyle.Flex;
        }
    }
    private void OnSoundUnmute()
    {
        if (MuteButton != null && UnmuteButton != null)
        {
            UnmuteButton.style.display = DisplayStyle.None;
            MuteButton.style.display = DisplayStyle.Flex;
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
}
