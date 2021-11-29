namespace MassTransit
{
    using System.Data;


    public interface IDapperSagaRepositoryConfigurator
    {
        IsolationLevel IsolationLevel { set; }
    }


    public interface IDapperSagaRepositoryConfigurator<TSaga> :
        IDapperSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }
}
