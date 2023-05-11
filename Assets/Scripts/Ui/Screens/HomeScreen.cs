using UnityEngine.UIElements;

public class HomeScreen : MenuScreen
{
    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        _showMenuBar = true;
    }

    private void OnEnable()
    {
        _mainMenuUIManager.ShowHomeScreen();
    }
}
