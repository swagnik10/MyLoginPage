using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MyApp.Mappings;
using NHibernate;
using NHibernate.Driver;

namespace MyApp.Infrastructure;
public static class NHibernateHelper
{
    private static ISessionFactory? _sessionFactory;
    private static string? _connectionString;

    public static void Configure(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static ISessionFactory SessionFactory
    {
        get
        {
            if (_sessionFactory == null)
            {
                _sessionFactory = Fluently.Configure()
                    .Database(
                        MsSqlConfiguration.MsSql2012
                            .ConnectionString(_connectionString)
                            .Driver<MicrosoftDataSqlClientDriver>()
                            .ShowSql()
                    )
                    .Mappings(m =>
                        m.FluentMappings
                            .AddFromAssemblyOf<UsersCredentialsMap>()
                    )
                    .BuildSessionFactory();
            }

            return _sessionFactory;
        }
    }
}
