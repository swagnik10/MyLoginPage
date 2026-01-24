namespace MyApp.Contract;

public class ApiResponse<T>
{
    public T? Data { get; set; }          // the actual object returned, null if none
    public string? Message { get; set; }   // informational message
    public bool Success { get; set; }     // true if request succeeded
}

