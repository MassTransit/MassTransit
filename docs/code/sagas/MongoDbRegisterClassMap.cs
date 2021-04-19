namespace MongoDbSagaRegisterClassMap
{
    using System;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using PersistedSaga;

    class OrderStateClassMap :
        BsonClassMap<OrderState>
    {
        public OrderStateClassMap()
        {
            MapProperty(x => x.OrderDate)
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        }
    }


    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            services.AddSingleton<BsonClassMap<OrderState>, OrderStateClassMap>();

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .MongoDbRepository(r =>
                    {
                        r.Connection = "mongodb://127.0.0.1";
                        r.DatabaseName = "orderdb";
                    });
            });
        }
    }
}
