namespace MassTransit.RabbitMqTransport.Tests
{
    using RabbitMQ.Client;


    public static class TestFactory
    {
        public static ConnectionFactory ConnectionFactory()
        {
            return new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "test",
                HostName = "localhost",
                Protocol = Protocols.AMQP_0_9_1
            };
        }
    }
}
