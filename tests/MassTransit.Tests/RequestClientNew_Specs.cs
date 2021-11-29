namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;
    using RequestClientMessages;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_the_new_request_client_syntax :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_awesome()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            Response<Value> response = await client.GetResponse<Value>(new GetValue());

            Assert.That(response.RequestId.HasValue, Is.True);
        }

        [Test]
        public async Task Should_be_awesome_with_a_side_of_sourdough_toast()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            Response<Value> response;
            using (RequestHandle<GetValue> request = client.Create(new GetValue()))
            {
                request.UseExecute(context => context.Headers.Set("Frank", "Mary"));

                response = await request.GetResponse<Value>();
            }

            Assert.That(response.RequestId.HasValue, Is.True);
            Assert.That(response.Headers.TryGetHeader("Frank", out var value), Is.True);
            Assert.That(value, Is.EqualTo("Mary"));
        }

        [Test]
        public async Task Should_throw_a_timeout_exception()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress, RequestTimeout.After(s: 1));

            Assert.That(async () => await client.GetResponse<Value>(new GetValue { Discard = true }), Throws.TypeOf<RequestTimeoutException>());
        }

        [Test]
        public async Task Should_throw_a_timeout_exception_with_milliseconds()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress, 100);

            Assert.That(async () => await client.GetResponse<Value>(new GetValue { Discard = true }), Throws.TypeOf<RequestTimeoutException>());
        }

        [Test]
        public async Task Should_throw_canceled_exception_when_already_canceled()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                Assert.That(async () => await client.GetResponse<Value>(new GetValue { Discard = true }, source.Token), Throws.TypeOf<TaskCanceledException>());
            }
        }

        [Test]
        public async Task Should_throw_canceled_exception_when_canceled()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            using (var source = new CancellationTokenSource(100))
            {
                Assert.That(async () => await client.GetResponse<Value>(new GetValue { Discard = true }, source.Token), Throws.TypeOf<TaskCanceledException>());
            }
        }

        [Test]
        public async Task Should_throw_the_request_exception()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            Assert.That(async () => await client.GetResponse<Value>(new GetValue { BlowUp = true }), Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_throw_the_request_exception_including_fault()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            try
            {
                await client.GetResponse<Value>(new GetValue { BlowUp = true });

                Assert.Fail("Should have thrown");
            }
            catch (RequestFaultException exception)
            {
                Assert.That(exception.Fault.Exceptions.First().ExceptionType, Is.EqualTo(TypeCache<IntentionalTestException>.ShortName));
                Assert.That(exception.RequestType, Is.EqualTo(TypeCache<GetValue>.ShortName));
                Assert.That(exception.Fault.FaultMessageTypes, Is.EqualTo(MessageTypeCache<GetValue>.MessageTypeNames));
            }
            catch
            {
                Assert.Fail("Unknown exception type thrown");
            }
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<GetValue>(configurator, async context =>
            {
                if (context.Message.Discard)
                    return;

                if (context.Message.BlowUp)
                    throw new IntentionalTestException("I hate it when this happens");

                await context.RespondAsync(new Value(), responseContext =>
                {
                    if (!responseContext.TryGetPayload(out ConsumeContext _))
                        throw new InvalidOperationException("Expected to find a ConsumeContext there");
                });
            });
        }
    }


    [TestFixture]
    public class Using_the_request_with_multiple_result_types :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_fault_at_the_request_if_the_consumer_faults_with_one_response_type()
        {
            IRequestClient<RegisterMember> client = Bus.CreateRequestClient<RegisterMember>(InputQueueAddress);

            Assert.That(async () => await client.GetResponse<MemberRegistered>(new RegisterMember { MemberId = "Logan5" }),
                Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_fault_at_the_request_if_the_consumer_faults_with_two_response_types()
        {
            IRequestClient<RegisterMember> client = Bus.CreateRequestClient<RegisterMember>(InputQueueAddress);

            Assert.That(async () => await client.GetResponse<MemberRegistered, ExistingMemberFound>(new RegisterMember { MemberId = "Logan5" }),
                Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_match_the_first_result()
        {
            IRequestClient<RegisterMember> client = Bus.CreateRequestClient<RegisterMember>(InputQueueAddress);

            Response<MemberRegistered, ExistingMemberFound> response = await client.GetResponse<MemberRegistered, ExistingMemberFound>(new RegisterMember());

            Assert.That(response.Is(out Response<MemberRegistered> _), Is.True, "Should have been registered");

            Assert.That(response.Is(out Response<ExistingMemberFound> _), Is.False, "Should not have been an existing member");
        }

        [Test]
        public async Task Should_match_the_second_result()
        {
            IRequestClient<RegisterMember> client = Bus.CreateRequestClient<RegisterMember>(InputQueueAddress);

            Response<MemberRegistered, ExistingMemberFound> response =
                await client.GetResponse<MemberRegistered, ExistingMemberFound>(new RegisterMember { MemberId = "Johnny5" });

            Assert.That(response.Is(out Response<MemberRegistered> _), Is.False, "Should not have been registered");

            Assert.That(response.Is(out Response<ExistingMemberFound> _), Is.True, "Should have been an existing member");
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<RegisterMember>(configurator, context =>
            {
                if (context.Message.MemberId == "Logan5")
                    throw new IntentionalTestException("Logon Ran");

                if (context.Message.MemberId == "Johnny5")
                    return context.RespondAsync<ExistingMemberFound>(new { context.Message.MemberId });

                return context.RespondAsync<MemberRegistered>(new { context.Message.MemberId });
            });
        }


        public class RegisterMember
        {
            public string MemberId { get; set; }
        }


        public class MemberRegistered
        {
            public string MemberId { get; set; }
        }


        public class ExistingMemberFound
        {
            public string MemberId { get; set; }
        }
    }


    [TestFixture]
    public class Using_the_request_client_with_cancellation_token :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_an_earlier_timeout()
        {
            var timeout = TimeSpan.FromMilliseconds(100);
            var ctsTimeout = TimeSpan.FromMilliseconds(140);

            var numOfLoops = 5;
            for (var i = 0; i < numOfLoops; ++i)
            {
                Console.WriteLine($"Sending request {i + 1} of {numOfLoops}");

                try
                {
                    using (var timeoutReq = new CancellationTokenSource(ctsTimeout))
                    {
                        IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, timeout);

                        await client.GetResponse<PongMessage>(new PingMessage(), timeoutReq.Token)
                            .OrTimeout(TimeSpan.FromSeconds(5))
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        [Test]
        public async Task Should_handle_cancellation()
        {
            var timeout = TimeSpan.FromMilliseconds(100);
            var ctsTimeout = TimeSpan.FromMilliseconds(100);

            var numOfLoops = 5;
            for (var i = 0; i < numOfLoops; ++i)
            {
                Console.WriteLine($"Sending request {i + 1} of {numOfLoops}");

                try
                {
                    using (var timeoutReq = new CancellationTokenSource(ctsTimeout))
                    {
                        IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, timeout);

                        await client.GetResponse<PongMessage>(new PingMessage(), timeoutReq.Token)
                            .OrTimeout(TimeSpan.FromSeconds(5))
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                await Task.Delay(500);
                await context.RespondAsync(new PongMessage(), responseContext =>
                {
                });
            });
        }
    }


    namespace RequestClientMessages
    {
        public class GetValue
        {
            public bool Discard { get; set; }
            public bool BlowUp { get; set; }
        }


        public class Value
        {
        }


        public class ReturnedValue
        {
        }


        public class AuditGetValue
        {
        }
    }


    [TestFixture]
    public class Sending_a_request_with_a_timeout :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_copy_the_time_to_live_to_the_response()
        {
            Assert.That(_response.ExpirationTime.HasValue, Is.True);
        }

        [Test]
        public async Task Should_not_copy_the_time_to_live_to_published_messages()
        {
            ConsumeContext<AuditGetValue> consumeContext = await _audited;

            Assert.That(consumeContext.ExpirationTime.HasValue, Is.False);
        }

        [Test]
        public async Task Should_not_copy_the_time_to_live_to_sent_messages()
        {
            ConsumeContext<ReturnedValue> consumeContext = await _returned;

            Assert.That(consumeContext.ExpirationTime.HasValue, Is.False);
        }

        Uri _auditEndpointAddress;
        Task<ConsumeContext<AuditGetValue>> _audited;
        Task<ConsumeContext<ReturnedValue>> _returned;
        Response<Value> _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            _response = await client.GetResponse<Value>(new GetValue());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("audit_queue", cfg =>
            {
                _audited = Handled<AuditGetValue>(cfg);
                _returned = Handled<ReturnedValue>(cfg);

                _auditEndpointAddress = cfg.InputAddress;
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<GetValue>(configurator, async context =>
            {
                if (context.Message.Discard)
                    return;

                if (context.Message.BlowUp)
                    throw new IntentionalTestException("I hate it when this happens");

                await context.Publish(new ReturnedValue());

                await context.Send(_auditEndpointAddress, new AuditGetValue());

                await context.RespondAsync(new Value(), responseContext =>
                {
                    if (!responseContext.TryGetPayload(out ConsumeContext _))
                        throw new InvalidOperationException("Expected to find a ConsumeContext there");
                });
            });
        }
    }


    [TestFixture]
    public class Sending_a_request_that_faults_with_a_timeout :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_copy_the_expiration_time_to_the_sent_fault()
        {
            Task<ConsumeContext<Fault<GetValue>>> handler = SubscribeHandler<Fault<GetValue>>();

            IRequestClient<GetValue> client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            try
            {
                await client.GetResponse<Value>(new GetValue { BlowUp = true });

                Assert.Fail("Should have thrown");
            }
            catch (RequestFaultException)
            {
            }

            ConsumeContext<Fault<GetValue>> consumeContext = await handler;

            Assert.That(consumeContext.ExpirationTime.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<GetValue>(configurator, async context =>
            {
                if (context.Message.Discard)
                    return;

                if (context.Message.BlowUp)
                    throw new IntentionalTestException("I hate it when this happens");

                await context.Publish(new ReturnedValue());


                await context.RespondAsync(new Value(), responseContext =>
                {
                    if (!responseContext.TryGetPayload(out ConsumeContext _))
                        throw new InvalidOperationException("Expected to find a ConsumeContext there");
                });
            });
        }
    }


    [TestFixture]
    public class Sending_a_request_through_the_outbox :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_skip_the_outbox()
        {
            IRequestClient<A> client = Bus.CreateRequestClient<A>(InputQueueAddress);

            await client.GetResponse<D>(new A());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.Consumer(() => new ConsumerA(Bus));
            configurator.Consumer(() => new ConsumerB());
        }


        class ConsumerA :
            IConsumer<A>
        {
            readonly IBus _bus;

            public ConsumerA(IBus bus)
            {
                _bus = bus;
            }

            public async Task Consume(ConsumeContext<A> context)
            {
                IRequestClient<B> client = context.CreateRequestClient<B>(_bus, context.ReceiveContext.InputAddress);

                await client.GetResponse<C>(new B());

                await context.RespondAsync(new D());
            }
        }


        class ConsumerB :
            IConsumer<B>
        {
            public async Task Consume(ConsumeContext<B> context)
            {
                await context.RespondAsync(new C());
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


        class D
        {
        }
    }
}
