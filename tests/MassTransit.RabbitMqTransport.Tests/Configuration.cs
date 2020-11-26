namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using NUnit.Framework;


    static class Configuration
    {
        public static string AmazonRabbitMqHost =>
            TestContext.Parameters.Exists(nameof(AmazonRabbitMqHost))
                ? TestContext.Parameters.Get(nameof(AmazonRabbitMqHost))
                : Environment.GetEnvironmentVariable("MT_AMQ_RMQ_HOST") ?? throw new ConfigurationException("Missing configuration");

        public static string AmazonRabbitMqUser =>
            TestContext.Parameters.Exists(nameof(AmazonRabbitMqUser))
                ? TestContext.Parameters.Get(nameof(AmazonRabbitMqUser))
                : Environment.GetEnvironmentVariable("MT_AMQ_RMQ_USER") ?? throw new ConfigurationException("Missing configuration");

        public static string AmazonRabbitMqPass =>
            TestContext.Parameters.Exists(nameof(AmazonRabbitMqPass))
                ? TestContext.Parameters.Get(nameof(AmazonRabbitMqPass))
                : Environment.GetEnvironmentVariable("MT_AMQ_RMQ_PASS") ?? throw new ConfigurationException("Missing configuration");
    }
}
