using System;

public class OperationEvent
{
    public static event Action MethodCalled;
    public static event Action<TaskResult<bool>> MethodFinished;

    public static void OnMethodFinished(TaskResult<bool> obj)
    {
        MethodFinished?.Invoke(obj);
    }

    public static void OnMethodCalled()
    {
        MethodCalled?.Invoke();
    }
}