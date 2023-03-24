using UnityEngine;

public class HomeScreen : MenuScreen
{
    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = true;
    }

    private void OnEnable()
    {
        ShowScreen();
    }
}
