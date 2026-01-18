namespace MyApp.Infrastructure;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    public IUnitOfWork Create()
    {
        return new UnitOfWork();
    }
}

