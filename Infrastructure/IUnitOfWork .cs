namespace MyApp.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    NHibernate.ISession Session { get; }
    void BeginTransaction();
    void Commit();
    void Rollback();
}