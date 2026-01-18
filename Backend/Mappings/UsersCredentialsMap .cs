using FluentNHibernate.Mapping;
using MyApp.Domain;

namespace MyApp.Mappings;

public class UsersCredentialsMap : ClassMap<UsersCredentials>
{
    public UsersCredentialsMap()
    {
        Table("auth.UsersCredentials");

        Id(x => x.UserId)
            .Column("UserId")
            .GeneratedBy.Identity();

        Map(x => x.UserName)
            .Not.Nullable()
            .Length(100);

        Map(x => x.Password)
            .Not.Nullable()
            .Length(255);

        Map(x => x.CreatedDate)
            .Not.Nullable();

        Map(x => x.UpdatedDate)
            .Not.Nullable();

        // Future improvement (tracked):
        // Unique constraint on UserName via mapping
    }
}

