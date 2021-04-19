namespace RedisSagaContainer
{
    using System;
    using MassTransit;
    using MassTransit.RedisIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using RedisSaga;

    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                const string configurationString = "127.0.0.1";

                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .RedisRepository(configurationString);
            });
        }
    }
}