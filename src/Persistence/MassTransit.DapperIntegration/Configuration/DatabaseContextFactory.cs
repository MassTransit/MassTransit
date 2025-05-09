namespace MassTransit
{
    using System.Data.Common;
    using DapperIntegration.Saga;

    public delegate DatabaseContext<TSaga> DatabaseContextFactory<TSaga>(DbConnection connection, DbTransaction transaction)
        where TSaga : class, ISaga;
}
