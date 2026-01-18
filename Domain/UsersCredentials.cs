namespace MyApp.Domain;
public class UsersCredentials
{
    public virtual int UserId { get; set; }
    public virtual string UserName { get; set; }
    public virtual string Password { get; set; }
    public virtual DateTime CreatedDate { get; set; }
    public virtual DateTime UpdatedDate { get; set; }
}
