public class TaskResult<T>
{
    public bool IsSuccess { get; set; }

    public bool IsFailure { get; set; }

    public string ErrorMessage { get; set; }
    public T Value { get; }

    public TaskResult(T value) => (IsSuccess, Value) = (true, value);
    public TaskResult(string errorMessage) => (IsFailure, ErrorMessage) = (true, errorMessage);
}