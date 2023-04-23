using UnityEngine.UIElements;

public class HomeScreen : MenuScreen
{

    private Label _welcomeLabel;
    
    private static string _welcomeLabelName = "WelcomeLabel";


    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        _welcomeLabel = _root.Q<Label>(_welcomeLabelName);
        _showMenuBar = true;
        SetupSizes();
    }
    
    private void SetupSizes()
    {
        var devicetype = Tools.GetDeviceType();

        if (devicetype == DeviceType.Phone)
        {
            _welcomeLabel.style.fontSize = new StyleLength(220);
        }
        else if(devicetype == DeviceType.Tablet)
        {
            _welcomeLabel.style.fontSize = new StyleLength(180);
        }
    }

    private void OnEnable()
    {
        ShowScreen();
    }
}
