namespace MassTransit.DapperIntegration
{
    using System.Data;
    using Saga;


    public interface IDapperSagaRepositoryConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        IsolationLevel IsolationLevel { set; }
    }
}
