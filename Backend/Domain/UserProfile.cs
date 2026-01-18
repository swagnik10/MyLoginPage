namespace MyApp.Domain;

public class UserProfile
{
    public virtual int ProfileId { get; set; }
    public virtual UsersCredentials User { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual string Address { get; set; }
    public virtual string PhoneNumber { get; set; }
    public virtual DateTime CreatedDate { get; set; }
    public virtual DateTime UpdatedDate { get; set; }
}
