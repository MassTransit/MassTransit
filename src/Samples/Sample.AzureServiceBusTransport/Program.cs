namespace Sample.AzureServiceBusTransport
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using GreenPipes;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.Extensions.Configuration;


    class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .AddJsonFile("appSettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration["AzureServiceBusConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Please provide connection string");
            }

            var serviceProvider = new ServiceCollection()
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                    configure.SetMinimumLevel(LogLevel.Debug);
                })
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var bus = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                var host = cfg.Host(connectionString, configurator =>
                {
                    configurator.OperationTimeout = TimeSpan.FromSeconds(5);
                });

                cfg.UseExtensionsLogging(loggerFactory);

                cfg.ReceiveEndpoint(host, "test_queue_v4", configurator =>
                {
                    configurator.Consumer<TestMessageConsumer>();
                });
            });

            var handle = await bus.StartAsync();

            while (true)
            {
                Console.WriteLine("Press enter to send message ");
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    await bus.Publish(new TestMessage
                    {
                        Name = "Hi"
                    });

                    Console.WriteLine("Send message ");
                }

                if (key.Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            Console.WriteLine("Stopping bus");

            await handle.StopAsync();

            Console.WriteLine("Stopped bus");
        }
    }
}
