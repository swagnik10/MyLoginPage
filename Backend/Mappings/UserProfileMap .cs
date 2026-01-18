using FluentNHibernate.Mapping;
using MyApp.Domain;

namespace MyApp.Mappings;
public class UserProfileMap : ClassMap<UserProfile>
{
    public UserProfileMap()
    {
        Table("auth.UserProfile");

        Id(x => x.ProfileId)
            .GeneratedBy.Identity();

        //References(x => x.User)
        //    .Column("UserId")
        //    .Not.Nullable()
        //    .Cascade.None();
        Map(x => x.UserId);

        Map(x => x.FirstName);
        Map(x => x.LastName);
        Map(x => x.Address);
        Map(x => x.PhoneNumber);

        Map(x => x.CreatedDate)
            .Not.Nullable();

        Map(x => x.UpdatedDate)
            .Not.Nullable();
    }
}

