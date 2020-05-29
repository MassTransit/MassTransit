namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace SubscriptionTests
    {
        using System.Threading.Tasks;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Using_a_subscription_endpoint :
            AzureServiceBusTestFixture
        {
            [Test]
            public async Task Should_succeed()
            {
                await Bus.Publish(new MessageA());
                await Bus.Publish(new MessageB());

                await _handledA;
                await _handledB;
            }

            Task<ConsumeContext<MessageA>> _handledA;
            Task<ConsumeContext<MessageB>> _handledB;

            protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                _handledA = Handled<MessageA>(configurator);
            }

            protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
            {
                configurator.SubscriptionEndpoint<MessageB>("phatboyg_you_know_me", x =>
                {
                    _handledB = Handled<MessageB>(x);
                });
            }
        }


        [TestFixture]
        public class Using_a_subscription_endpoint_that_faults :
            AzureServiceBusTestFixture
        {
            [Test]
            public async Task Should_succeed()
            {
                await Bus.Publish(new MessageC());
                await Bus.Publish(new MessageB());

                await _handledA;

                await Task.Delay(3000);
            }

            Task<ConsumeContext<MessageA>> _handledA;
            Task<ConsumeContext<MessageB>> _handledB;

            protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                _handledA = Handled<MessageA>(configurator);
            }

            protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
            {
                configurator.SubscriptionEndpoint<MessageB>("phatboyg_you_know_me", x =>
                {
                    _handledB = Handler<MessageB>(x, async context =>
                    {
                        await context.Publish(new MessageA());

                        throw new IntentionalTestException("Oh, dang.");
                    });
                });

                configurator.SubscriptionEndpoint<MessageC>("no_handlers", x =>
                {
                });
            }
        }


        public class MessageA
        {
            public string Value { get; set; }
        }


        public class MessageB
        {
            public string Value { get; set; }
        }


        public class MessageC
        {
            public string Value { get; set; }
        }


        public class ResponseA
        {
            public string Value { get; set; }
        }


        public class ResponseB
        {
            public string Value { get; set; }
        }


        [TestFixture]
        public class Using_multiple_subscription_endpoints :
            AzureServiceBusTestFixture
        {
            [Test]
            public async Task Should_handle_a_message_per_endpoint()
            {
                _responseA = SubscribeHandler<ResponseA>();
                _responseB = SubscribeHandler<ResponseB>();

                await Bus.Publish(new MessageA(), context => context.ResponseAddress = Bus.Address);
                await Bus.Publish(new MessageB(), context => context.ResponseAddress = Bus.Address);

                await _responseA;
                await _responseB;
            }

            Task<ConsumeContext<ResponseA>> _responseA;
            Task<ConsumeContext<ResponseB>> _responseB;

            protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
            {
                configurator.SubscriptionEndpoint<MessageA>("phatboyg_you_know_me", x =>
                {
                    x.Consumer<Consumer>();
                });

                configurator.SubscriptionEndpoint<MessageB>("phatboyg_you_know_me", x =>
                {
                    x.Consumer<Consumer>();
                });
            }


            public class Consumer :
                IConsumer<MessageA>,
                IConsumer<MessageB>
            {
                public Task Consume(ConsumeContext<MessageA> context)
                {
                    return context.RespondAsync(new ResponseA {Value = context.Message.Value});
                }

                public Task Consume(ConsumeContext<MessageB> context)
                {
                    return context.RespondAsync(new ResponseB {Value = context.Message.Value});
                }
            }
        }
    }
}
