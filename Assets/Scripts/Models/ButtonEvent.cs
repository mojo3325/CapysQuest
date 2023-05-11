using System;

public class ButtonEvent
{
    public static event Action OpenMenuCalled;
    public static event Action CloseMenuCalled;
    public static event Action EnterButtonCalled;

    public static void OnOpenMenuCalled()
    {
        OpenMenuCalled?.Invoke();
    }

    public static void OnEnterButtonCalled()
    {
        EnterButtonCalled?.Invoke();
    }

    public static void OnCloseMenuCalled()
    {
        CloseMenuCalled?.Invoke();
    }
}
