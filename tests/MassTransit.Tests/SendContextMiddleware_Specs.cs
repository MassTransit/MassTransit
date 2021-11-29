namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using MassTransit.Testing;
    using MassTransit.Testing.Implementations;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Adding_a_send_context_middleware_component :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_consume_context_available()
        {
            Task<ConsumeContext<C>> handler = await ConnectPublishHandler<C>();

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


    [TestFixture]
    public class Accessing_payload_from_consume_context_in_send_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_contain_the_same_payloads()
        {
            EndpointConvention.Map<B>(InputQueueAddress);

            var sendObserver = new BusTestSendObserver(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1), BusTestHarness.InactivityToken);

            using (Bus.ConnectSendObserver(sendObserver))
            {
                await InputQueueSendEndpoint.Send(new A());

                ISentMessage<A> a = sendObserver.Messages.Select<A>().FirstOrDefault();
                ISentMessage<B> b = sendObserver.Messages.Select<B>().FirstOrDefault();

                a.ShouldNotBeNull();
                b.ShouldNotBeNull();

                Dictionary<string, object> ah = a.Context.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value);
                Dictionary<string, object> bh = b.Context.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value);

                ah.ShouldContainKey("x-send-filter");
                ah.ShouldContainKey("x-send-message-filter");
                ah["x-send-filter"].ShouldBe("send-filter");
                ah["x-send-message-filter"].ShouldBe("send-message-filter");

                bh.ShouldContainKey("x-send-filter");
                bh.ShouldContainKey("x-send-message-filter");

                // those fails, as while they DO have ",has-consume-context" they don't have access to SomePayload
                bh["x-send-filter"].ShouldBe("send-filter,has-consume-context,has-some-payload:hello");
                bh["x-send-message-filter"].ShouldBe("send-message-filter,has-consume-context,has-some-payload:hello");
            }
        }


        class MyConsumeFilter<TContext> : IFilter<TContext>
            where TContext : class, ConsumeContext
        {
            public async Task Send(TContext context, IPipe<TContext> next)
            {
                context.GetOrAddPayload(() => new SomePayload("hello"));

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-consume-filter");
            }
        }


        class MyConsumeMessageFilter<TContext, TMessage> : IFilter<TContext>
            where TContext : class, ConsumeContext<TMessage>
            where TMessage : class
        {
            public async Task Send(TContext context, IPipe<TContext> next)
            {
                context.GetOrAddPayload(() => new SomePayload("hello"));

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-consume-message-filter");
            }
        }


        class MySendFilter : IFilter<SendContext>
        {
            public async Task Send(SendContext context, IPipe<SendContext> next)
            {
                var carrier = "send-filter";

                if (context.TryGetPayload(out ConsumeContext cc))
                {
                    // should occur on message B and it DOES, all good

                    carrier += ",has-consume-context";

                    if (cc.TryGetPayload(out SomePayload ccsp)) // never occurs, NOT GOOD :(
                        carrier += ",has-some-payload:" + ccsp.Text;
                }

                context.Headers.Set("x-send-filter", carrier);

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-send-filter");
            }
        }


        class MySendMessageFilter<TMessage> : IFilter<SendContext<TMessage>>
            where TMessage : class
        {
            public async Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
            {
                var carrier = "send-message-filter";

                if (context.TryGetPayload(out ConsumeContext cc))
                {
                    // should occur on message B and it DOES, all good

                    carrier += ",has-consume-context";

                    if (cc.TryGetPayload(out SomePayload ccsp)) // never occurs, NOT GOOD :(
                        carrier += ",has-some-payload:" + ccsp.Text;
                }

                context.Headers.Set("x-send-message-filter", carrier);

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-send-message-filter");
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
                return Task.CompletedTask;
            }
        }


        class A
        {
        }


        class B
        {
        }


        class SomePayload
        {
            public SomePayload(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var observer = new MyConsumePipeSpecObserver();

            configurator.ConnectConsumerConfigurationObserver(observer);

            configurator.ConfigureSend(spc =>
            {
                spc.AddPipeSpecification(new MySendPipeSpec());
                spc.ConnectSendPipeSpecificationObserver(new MySendPipeSpecObserver());
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<ConsumerA>();
            configurator.Consumer<ConsumerB>();
        }


        class MyConsumePipeSpecObserver : IConsumerConfigurationObserver
        {
            public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
                where TConsumer : class
            {
                configurator.AddPipeSpecification(new MyConsumePipeSpec<TConsumer>());
            }

            public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
                where TConsumer : class
                where TMessage : class
            {
                configurator.AddPipeSpecification(new MyConsumeMessagePipeSpec<TConsumer, TMessage>());
            }
        }


        class MyConsumePipeSpec<TConsumer> : IPipeSpecification<ConsumerConsumeContext<TConsumer>>
            where TConsumer : class
        {
            public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
            {
                builder.AddFilter(new MyConsumeFilter<ConsumerConsumeContext<TConsumer>>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }


        class MyConsumeMessagePipeSpec<TConsumer, TMessage> : IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
            where TConsumer : class
            where TMessage : class
        {
            public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
            {
                builder.AddFilter(new MyConsumeMessageFilter<ConsumerConsumeContext<TConsumer, TMessage>, TMessage>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }


        class MySendPipeSpecObserver : ISendPipeSpecificationObserver
        {
            public void MessageSpecificationCreated<TMessage>(IMessageSendPipeSpecification<TMessage> specification)
                where TMessage : class
            {
                specification.AddPipeSpecification(new MySendMessagePipeSpec<TMessage>());
            }
        }


        class MySendPipeSpec : IPipeSpecification<SendContext>
        {
            public void Apply(IPipeBuilder<SendContext> builder)
            {
                builder.AddFilter(new MySendFilter());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }


        class MySendMessagePipeSpec<TMessage> : IPipeSpecification<SendContext<TMessage>>
            where TMessage : class
        {
            public void Apply(IPipeBuilder<SendContext<TMessage>> builder)
            {
                builder.AddFilter(new MySendMessageFilter<TMessage>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }
    }


    [TestFixture]
    public class Accessing_payload_from_consume_context_in_publish_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_contain_the_same_payloads()
        {
            var publishObserver = new BusTestPublishObserver(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1), BusTestHarness.InactivityToken);

            using (Bus.ConnectPublishObserver(publishObserver))
            {
                await Bus.Publish(new A());

                IPublishedMessage<A> a = publishObserver.Messages.Select<A>().FirstOrDefault();
                IPublishedMessage<B> b = publishObserver.Messages.Select<B>().FirstOrDefault();

                a.ShouldNotBeNull();
                b.ShouldNotBeNull();

                Dictionary<string, object> ah = a.Context.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value);
                Dictionary<string, object> bh = b.Context.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value);

                ah.ShouldContainKey("x-send-filter");
                ah.ShouldContainKey("x-send-message-filter");
                ah["x-send-filter"].ShouldBe("send-filter");
                ah["x-send-message-filter"].ShouldBe("send-message-filter");

                bh.ShouldContainKey("x-send-filter");
                bh.ShouldContainKey("x-send-message-filter");

                // those fails, as while they DO have ",has-consume-context" they don't have access to SomePayload
                bh["x-send-filter"].ShouldBe("send-filter,has-consume-context,has-some-payload:hello");
                bh["x-send-message-filter"].ShouldBe("send-message-filter,has-consume-context,has-some-payload:hello");
            }
        }


        class MyConsumeFilter<TContext> : IFilter<TContext>
            where TContext : class, ConsumeContext
        {
            public async Task Send(TContext context, IPipe<TContext> next)
            {
                context.GetOrAddPayload(() => new SomePayload("hello"));

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-consume-filter");
            }
        }


        class MyConsumeMessageFilter<TContext, TMessage> : IFilter<TContext>
            where TContext : class, ConsumeContext<TMessage>
            where TMessage : class
        {
            public async Task Send(TContext context, IPipe<TContext> next)
            {
                context.GetOrAddPayload(() => new SomePayload("hello"));

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-consume-message-filter");
            }
        }


        class MyPublishFilter : IFilter<PublishContext>
        {
            public async Task Send(PublishContext context, IPipe<PublishContext> next)
            {
                var carrier = "send-filter";

                if (context.TryGetPayload(out ConsumeContext cc))
                {
                    // should occur on message B and it DOES, all good

                    carrier += ",has-consume-context";

                    if (cc.TryGetPayload(out SomePayload ccsp)) // never occurs, NOT GOOD :(
                        carrier += ",has-some-payload:" + ccsp.Text;
                }

                context.Headers.Set("x-send-filter", carrier);

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-send-filter");
            }
        }


        class MyPublishMessageFilter<TMessage> : IFilter<PublishContext<TMessage>>
            where TMessage : class
        {
            public async Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
            {
                var carrier = "send-message-filter";

                if (context.TryGetPayload(out ConsumeContext cc))
                {
                    // should occur on message B and it DOES, all good

                    carrier += ",has-consume-context";

                    if (cc.TryGetPayload(out SomePayload ccsp)) // never occurs, NOT GOOD :(
                        carrier += ",has-some-payload:" + ccsp.Text;
                }

                context.Headers.Set("x-send-message-filter", carrier);

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("my-send-message-filter");
            }
        }


        class ConsumerA : IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.Publish(new B());
            }
        }


        class ConsumerB : IConsumer<B>
        {
            public Task Consume(ConsumeContext<B> context)
            {
                return Task.CompletedTask;
            }
        }


        class A
        {
        }


        class B
        {
        }


        class SomePayload
        {
            public SomePayload(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var observer = new MyConsumePipeSpecObserver();

            configurator.ConnectConsumerConfigurationObserver(observer);

            configurator.ConfigurePublish(spc =>
            {
                spc.AddPipeSpecification(new MyPublishPipeSpec());
                spc.ConnectPublishPipeSpecificationObserver(new MyPublishPipeSpecObserver());
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<ConsumerA>();
            configurator.Consumer<ConsumerB>();
        }


        class MyConsumePipeSpecObserver : IConsumerConfigurationObserver
        {
            public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
                where TConsumer : class
            {
                configurator.AddPipeSpecification(new MyConsumePipeSpec<TConsumer>());
            }

            public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
                where TConsumer : class
                where TMessage : class
            {
                configurator.AddPipeSpecification(new MyConsumeMessagePipeSpec<TConsumer, TMessage>());
            }
        }


        class MyConsumePipeSpec<TConsumer> : IPipeSpecification<ConsumerConsumeContext<TConsumer>>
            where TConsumer : class
        {
            public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
            {
                builder.AddFilter(new MyConsumeFilter<ConsumerConsumeContext<TConsumer>>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }


        class MyConsumeMessagePipeSpec<TConsumer, TMessage> : IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
            where TConsumer : class
            where TMessage : class
        {
            public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
            {
                builder.AddFilter(new MyConsumeMessageFilter<ConsumerConsumeContext<TConsumer, TMessage>, TMessage>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }


        class MyPublishPipeSpecObserver : IPublishPipeSpecificationObserver
        {
            public void MessageSpecificationCreated<TMessage>(IMessagePublishPipeSpecification<TMessage> specification)
                where TMessage : class
            {
                specification.AddPipeSpecification(new MyPublishMessagePipeSpec<TMessage>());
            }
        }


        class MyPublishPipeSpec : IPipeSpecification<PublishContext>
        {
            public void Apply(IPipeBuilder<PublishContext> builder)
            {
                builder.AddFilter(new MyPublishFilter());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }


        class MyPublishMessagePipeSpec<TMessage> : IPipeSpecification<PublishContext<TMessage>>
            where TMessage : class
        {
            public void Apply(IPipeBuilder<PublishContext<TMessage>> builder)
            {
                builder.AddFilter(new MyPublishMessageFilter<TMessage>());
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }
    }
}
