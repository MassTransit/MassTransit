namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using ConsumerBind_Specs;
    using NUnit.Framework;
    using RabbitMQ.Client;


    [TestFixture]
    public class Binding_an_additional_exchange :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri($"exchange:{_boundExchange}"));

            await endpoint.Send(new A());

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;
        readonly string _boundExchange = "bound-exchange";

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);

            configurator.Bind(_boundExchange);
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            await channel.ExchangeDeleteAsync(_boundExchange);
        }
    }


    [TestFixture]
    public class Binding_an_additional_exchange_with_an_additional_exchange :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri($"exchange:{ReboundExchange}"));

            await endpoint.Send(new A());

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;
        const string BoundExchange = "bound-exchange";
        const string ReboundExchange = "rebound-exchange";

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);

            configurator.Bind(BoundExchange, x => x.Bind(ReboundExchange));
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            await channel.ExchangeDeleteAsync(BoundExchange);
            await channel.ExchangeDeleteAsync(ReboundExchange);
        }
    }


    [TestFixture]
    public class Binding_an_additional_exchange_with_routing_key :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message()
        {
            var sendAddress = Bus.GetRabbitMqHostTopology().GetDestinationAddress(BoundExchange, x =>
            {
                x.Durable = false;
                x.AutoDelete = true;
                x.ExchangeType = ExchangeType.Direct;
            });

            var endpoint = await Bus.GetSendEndpoint(sendAddress);

            await endpoint.Send(new A(), x => x.SetRoutingKey("bondage"));

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;
        const string BoundExchange = "bound-exchange";

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);

            configurator.Bind(BoundExchange, x =>
            {
                x.Durable = false;
                x.AutoDelete = true;
                x.ExchangeType = ExchangeType.Direct;
                x.RoutingKey = "bondage";
            });
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            await channel.ExchangeDeleteAsync(BoundExchange);
        }
    }
}
