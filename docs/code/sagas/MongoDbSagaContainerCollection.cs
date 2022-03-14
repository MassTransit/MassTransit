namespace MongoDbSaga
{
    using System;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using PersistedSaga;

    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .MongoDbRepository(r =>
                    {
                        r.Connection = "mongodb://127.0.0.1";
                        r.DatabaseName = "orderdb";

                        r.CollectionName = "orders";
                    });
            });
        }
    }
}
