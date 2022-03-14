namespace RedisSagaContainerConfiguration
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
                const string configurationString = "127.0.0.1";

                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .RedisRepository(r =>
                    {
                        r.DatabaseConfiguration(configurationString);

                        // Default is Optimistic
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

                        // Optional, prefix each saga instance key with the string specified
                        // resulting dev:c6cfd285-80b2-4c12-bcd3-56a00d994736
                        r.KeyPrefix = "dev";

                        // Optional, to customize the lock key
                        r.LockSuffix = "-lockage";

                        // Optional, the default is 30 seconds
                        r.LockTimeout = TimeSpan.FromSeconds(90);
                    });;
            });
        }
    }
}
