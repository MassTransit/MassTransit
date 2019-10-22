namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using GreenPipes;
    using NUnit.Framework;


    [TestFixture]
    public class Configure_Specs
    {
        [Test]
        public void Should_fail_with_invalid_middleware()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                        h.RequestedConnectionTimeout(2000);
                    });

                    x.UseRetry(r =>
                    {
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_when_late_configuration_happens()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        var inputAddress = e.InputAddress;

                        e.Durable = false;
                        e.AutoDelete = true;
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_invalid_middleware_on_endpoint()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        e.UseRetry(r =>
                        {
                        });
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_empty_queue_name()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.OverrideDefaultBusEndpointQueueName("");
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_invalid_queue_name()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.ReceiveEndpoint(host, "0(*!)@((*#&!(*&@#/", e =>
                    {
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_not_fail_with_warnings()
        {
            Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                {
                });

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PurgeOnStartup = true;
                });
            });
        }
    }
}
