namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_priority_queue :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_allow_priority_to_be_specified()
        {
            await Bus.Request<PingMessage, PongMessage>(_endpointAddress, new PingMessage(), TestCancellationToken, TestTimeout, x =>
            {
                x.SetPriority(2);
            });
        }

        Uri _endpointAddress;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("priority_input_queue", x =>
            {
                x.ConfigureConsumeTopology = false;

                x.EnablePriority(4);

                _endpointAddress = x.InputAddress;

                x.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
            });
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            model.ExchangeDelete("priority_input_queue");
            model.QueueDelete("priority_input_queue");
        }
    }
}
