namespace AmazonRabbitMqConsoleListener
{
    using System;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("amqps://b-12345678-1234-1234-1234-123456789012.mq.us-east-2.amazonaws.com:5671"), h =>
                {
                    h.Username("username");
                    h.Password("password");
                });
            });
        }
    }
}
