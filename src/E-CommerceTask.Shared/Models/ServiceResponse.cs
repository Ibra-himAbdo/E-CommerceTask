namespace E_CommerceTask.Shared.Models;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ServiceResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ServiceResponse<T> { Data = data, IsSuccess = true, Message = message };
    }

    public static ServiceResponse<T> Failure(string message)
    {
        return new ServiceResponse<T> { IsSuccess = false, Message = message };
    }
}