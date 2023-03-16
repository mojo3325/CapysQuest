using Assets.SimpleLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUiManager : MonoBehaviour
{
    private VisualElement Fish1;
    private VisualElement Fish2;
    private VisualElement Fish3;
    private VisualElement Fish4;
    private VisualElement Fish5;
    private Button RightTapButton;
    private Button LeftTapButton;

    private Label GameText;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Fish1 = root.Q<VisualElement>("Fish1");
        Fish2 = root.Q<VisualElement>("Fish2");
        Fish3 = root.Q<VisualElement>("Fish3");
        Fish4 = root.Q<VisualElement>("Fish4");
        Fish5 = root.Q<VisualElement>("Fish5");

        GameText = root.Q<Label>("GameText");

        RightTapButton = root.Q<Button>("RightTapButton");
        LeftTapButton = root.Q<Button>("LeftTapButton");

        RightTapButton.clicked += () => EventManager.OnRightTap.Invoke();
        LeftTapButton.clicked += () => EventManager.OnLeftTap.Invoke();

        EventManager.OnTimeChange.AddListener(SetupTime);
        EventManager.OnTimeLost.AddListener(ShowTimeLostText);
        EventManager.OnTimeClimed.AddListener(ShowTimeReachedText);
        EventManager.OnZoneAchieved.AddListener(ShowZoneReachedText);

        LocalizationManager.Read();
    }

    private void ShowTimeLostText()
    {
        StartCoroutine(ShowTimeLost());
    }

    private void ShowZoneReachedText(ZoneType zoneType)
    {
        StartCoroutine(ShowZoneReached(zoneType));
    }

    private void ShowTimeReachedText()
    {
        StartCoroutine(ShowTimeReached());
    }

    IEnumerator ShowTimeLost()
    {
        var text = LocalizationManager.Localize("time_lost");
        GameText.style.display = DisplayStyle.Flex;
        GameText.text = text;
        yield return new WaitForSeconds(1.5f);
        GameText.style.display = DisplayStyle.None;
    }

    IEnumerator ShowZoneReached(ZoneType zoneType)
    {
        var text = LocalizationManager.Localize(zoneType.ToString());
        GameText.style.display = DisplayStyle.Flex;
        GameText.text = text;
        yield return new WaitForSeconds(1.5f);
        GameText.style.display = DisplayStyle.None;
    }

    IEnumerator ShowTimeReached()
    {
        var text = LocalizationManager.Localize("time_booster");
        GameText.style.display = DisplayStyle.Flex;
        GameText.text = text;
        yield return new WaitForSeconds(1.5f);
        GameText.style.display = DisplayStyle.None;
    }
    private void SetupTime(float time)
    {
        Fish1.style.display = time < 1 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish2.style.display = time < 2 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish3.style.display = time < 3 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish4.style.display = time < 4 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish5.style.display = time < 5 ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
