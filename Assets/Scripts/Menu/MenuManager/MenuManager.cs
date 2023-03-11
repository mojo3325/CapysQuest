using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;



public class MenuManager : MonoBehaviour
{

    private VisualElement MenuUi;
    private VisualElement GameUi;


    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        MenuUi = root.Q<VisualElement>("MenuUi");
        GameUi = root.Q<VisualElement>("GameUi");

        EventManager.OnCapyDie.AddListener(OnCapyDie);
        EventManager.OnPlayClick.AddListener(OnPlayClick);
    }
    private void OnCapyDie(DieType dieType)
    {
        StartCoroutine(ShowGameOverAferDie());
    }

    IEnumerator ShowGameOverAferDie()
    {
        yield return new WaitForSeconds(1.5f);
        GameUi.style.display = DisplayStyle.None;
        MenuUi.style.display = DisplayStyle.Flex;
    }

    private void OnPlayClick()
    {
        MenuUi.style.display = DisplayStyle.None;
        GameUi.style.display = DisplayStyle.Flex;
    }
}
