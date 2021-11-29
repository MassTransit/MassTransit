namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;


    [TestFixture]
    public class AlternateExchange_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_create_and_bind_the_exchange_and_properties()
        {
            await Bus.Publish<TheWorldImploded>(new { Value = "Whoa!" });

            await _handled;
        }

        Task<ConsumeContext<TheWorldImploded>> _handled;

        const string AlternateExchangeName = "publish-not-delivered";
        const string AlternateQueueName = "world-examiner";

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.PublishTopology.GetMessageTopology<TheWorldImploded>()
                .BindAlternateExchangeQueue(AlternateExchangeName);

            configurator.ReceiveEndpoint(AlternateQueueName, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Bind(AlternateExchangeName);

                _handled = Handled<TheWorldImploded>(x);
            });
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            base.OnCleanupVirtualHost(model);

            model.ExchangeDelete(AlternateExchangeName);
            model.QueueDelete(AlternateExchangeName);

            model.ExchangeDelete(AlternateQueueName);
            model.QueueDelete(AlternateQueueName);
        }


        public interface TheWorldImploded
        {
            string Value { get; }
        }
    }
}
