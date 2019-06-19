namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using SendPipeSpecifications;
    using TestFramework;


    [TestFixture]
    public class Adding_a_send_context_middleware_component :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_consume_context_available()
        {
            var handler = ConnectPublishHandler<C>();

            EndpointConvention.Map<B>(InputQueueAddress);

            await InputQueueSendEndpoint.Send(new A());

            ConsumeContext<C> handledC = await handler;
        }


        class MySendFilter<TMessage> : IFilter<SendContext<TMessage>>
            where TMessage : class
        {
            public async Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
            {
                var msg = context.Message;

                if (context.TryGetPayload(out ConsumeContext cc))
                {
                    if (typeof(TMessage) == typeof(A))
                        throw new InvalidOperationException("The A type should not be included");
                }
                else
                {
                    if (typeof(TMessage) == typeof(B))
                        throw new InvalidOperationException("The B type should  be included");
                }

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-send-filter");
            }
        }


        class ConsumerA : IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.Send(new B());
            }
        }


        class ConsumerB : IConsumer<B>
        {
            public Task Consume(ConsumeContext<B> context)
            {
                return context.Publish(new C());
            }
        }


        class A
        {
        }


        class B
        {
        }


        class C
        {
        }


        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(spc =>
            {
                spc.ConnectSendPipeSpecificationObserver(new MySendPipeSpecObserver());
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<ConsumerA>();
            configurator.Consumer<ConsumerB>();
        }


        class MySendPipeSpecObserver : ISendPipeSpecificationObserver
        {
            public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
                where T : class
            {
                specification.AddPipeSpecification(new MySendPipeSpec<T>());
            }
        }


        class MySendPipeSpec<TMessage> : IPipeSpecification<SendContext<TMessage>>
            where TMessage : class
        {
            public void Apply(IPipeBuilder<SendContext<TMessage>> builder)
            {
                builder.AddFilter(new MySendFilter<TMessage>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }
    }
}
