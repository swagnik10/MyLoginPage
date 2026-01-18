using NHibernate;

namespace MyApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly NHibernate.ISession _session;
    private ITransaction? _transaction;

    public NHibernate.ISession Session => _session;

    public UnitOfWork(NHibernate.ISession session)
    {
        _session = session;
    }

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
            _transaction?.Commit();
        }
    }

    public void Rollback()
    {
        if (_transaction != null && _transaction.IsActive)
        {
            _transaction?.Rollback();
        }
    }

    public void Dispose()
    {
        if (_transaction != null && _transaction.IsActive)
        {
            _transaction?.Dispose();
        }
    }
}
