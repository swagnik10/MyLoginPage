using NHibernate;

namespace MyApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly NHibernate.ISession _session;
    private ITransaction _transaction;

    public UnitOfWork()
    {
        _session = NHibernateHelper.SessionFactory.OpenSession();
    }

    public NHibernate.ISession Session => _session;

    public void BeginTransaction()
    {
        if (_transaction == null || !_transaction.IsActive)
        {
            _transaction = _session.BeginTransaction();
        }
    }

    public void Commit()
    {
        if (_transaction != null && _transaction.IsActive)
        {
            _transaction.Commit();
        }
    }

    public void Rollback()
    {
        if (_transaction != null && _transaction.IsActive)
        {
            _transaction.Rollback();
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _session?.Dispose();
    }
}
