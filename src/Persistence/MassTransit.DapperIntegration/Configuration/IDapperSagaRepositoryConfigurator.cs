namespace MassTransit.DapperIntegration
{
    using System.Data;
    using Saga;


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
