using UnityEngine;

public class HomeScreen : MenuScreen
{
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
    }

    private void OnEnable()
    {
        ShowScreen();
    }
}
