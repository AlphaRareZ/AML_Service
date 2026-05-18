namespace Presentation;

public class ApiResponse<T>
{
    public required bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
}