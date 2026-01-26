namespace MyApp.DTOs;

public class LoginResponse
{
    public int UserId { get; set; }
    public bool HasProfile { get; set; }
    public string AccessToken { get; set; }

}

