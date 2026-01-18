namespace MyApp.Infrastructure;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
