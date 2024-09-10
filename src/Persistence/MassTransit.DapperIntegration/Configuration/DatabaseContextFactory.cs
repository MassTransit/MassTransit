namespace MassTransit;

using System.Data;
using DapperIntegration.Saga;


public delegate DatabaseContext<TSaga> DatabaseContextFactory<TSaga>(IDbConnection connection, IDbTransaction transaction)
    where TSaga : class, ISaga;
