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
    private Label BackgroundGameText;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Fish1 = root.Q<VisualElement>("Fish1");
        Fish2 = root.Q<VisualElement>("Fish2");
        Fish3 = root.Q<VisualElement>("Fish3");
        Fish4 = root.Q<VisualElement>("Fish4");
        Fish5 = root.Q<VisualElement>("Fish5");

        GameText = root.Q<Label>("GameText");
        BackgroundGameText = root.Q<Label>("BackgroundGameText");

        RightTapButton = root.Q<Button>("RightTapButton");
        LeftTapButton = root.Q<Button>("LeftTapButton");

        RightTapButton.clicked += () => EventManager.OnRightTap.Invoke();
        LeftTapButton.clicked += () => EventManager.OnLeftTap.Invoke();

        EventManager.OnTimeChange.AddListener(ChangeTime);
        EventManager.OnTimeLost.AddListener(ShowTimeLostText);
        EventManager.OnPointReached.AddListener(ShowPointReachedText);
    }

    private void ShowTimeLostText()
    {
        StartCoroutine(ShowTimeLost());
    }

    private void ShowPointReachedText()
    {
        StartCoroutine(ShowPointReached());
    }

    IEnumerator ShowTimeLost()
    {
        BackgroundGameText.style.display = DisplayStyle.Flex;
        GameText.text = "Время вышло";
        BackgroundGameText.text = "Время вышло";
        yield return new WaitForSeconds(1.5f);
        BackgroundGameText.style.display = DisplayStyle.None;
    }

    IEnumerator ShowPointReached()
    {
        BackgroundGameText.style.display = DisplayStyle.Flex;
        GameText.text = "Чекпоинт";
        BackgroundGameText.text = "Чекпоинт";
        yield return new WaitForSeconds(1.5f);
        BackgroundGameText.style.display = DisplayStyle.None;
    }
    private void ChangeTime(float time)
    {
        Fish1.style.display = time < 1 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish2.style.display = time < 2 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish3.style.display = time < 3 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish4.style.display = time < 4 ? DisplayStyle.None : DisplayStyle.Flex;
        Fish5.style.display = time < 5 ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
