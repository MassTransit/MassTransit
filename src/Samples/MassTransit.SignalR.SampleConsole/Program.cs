namespace MassTransit.SignalR.SampleConsole
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging.Abstractions;
    using Transactions;


    static class Program
    {
        internal static async Task Main(string[] args)
        {
            //IReadOnlyList<IHubProtocol> protocols = new IHubProtocol[] { new JsonHubProtocol() };
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            // Important! The bus must be started before using it!
            await busControl.StartAsync();

            do
            {
                Console.WriteLine("Enter message (or quit to exit)");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                var test = new TransactionOutbox(busControl, busControl, new NullLoggerFactory());

                await test.Publish<MyMessage>(new {Name = "John"});
            }
            while (true);

            await busControl.StopAsync();
        }
    }


    public class MyMessage
    {
        public string Name { get; set; }
    }
}
