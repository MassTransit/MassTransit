namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Metadata;
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
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            Response<Value> response = await client.GetResponse<Value>(new GetValue());

            Assert.That(response.RequestId.HasValue, Is.True);
        }

        [Test]
        public async Task Should_be_awesome_with_a_side_of_sourdough_toast()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            Response<Value> response;
            using (RequestHandle<GetValue> request = client.Create(new GetValue()))
            {
                request.UseExecute(context => context.Headers.Set("Frank", "Mary"));

                response = await request.GetResponse<Value>();
            }

            Assert.That(response.RequestId.HasValue, Is.True);
            Assert.That(response.Headers.TryGetHeader("Frank", out object value), Is.True);
            Assert.That(value, Is.EqualTo("Mary"));
        }

        [Test]
        public async Task Should_throw_the_request_exception()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            Assert.That(async () => await client.GetResponse<Value>(new GetValue {BlowUp = true}), Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_throw_the_request_exception_including_fault()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            try
            {
                await client.GetResponse<Value>(new GetValue {BlowUp = true});

                Assert.Fail("Should have thrown");
            }
            catch (RequestFaultException exception)
            {
                Assert.That(exception.Fault.Exceptions.First().ExceptionType, Is.EqualTo(TypeMetadataCache<IntentionalTestException>.ShortName));
                Assert.That(exception.RequestType, Is.EqualTo(TypeMetadataCache<GetValue>.ShortName));
                Assert.That(exception.Fault.FaultMessageTypes, Is.EqualTo(TypeMetadataCache<GetValue>.MessageTypeNames));
            }
            catch
            {
                Assert.Fail("Unknown exception type thrown");
            }
        }

        [Test]
        public async Task Should_throw_a_timeout_exception()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress, RequestTimeout.After(s: 1));

            Assert.That(async () => await client.GetResponse<Value>(new GetValue {Discard = true}), Throws.TypeOf<RequestTimeoutException>());
        }

        [Test]
        public async Task Should_throw_a_timeout_exception_with_milliseconds()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress, 100);

            Assert.That(async () => await client.GetResponse<Value>(new GetValue {Discard = true}), Throws.TypeOf<RequestTimeoutException>());
        }

        [Test]
        public async Task Should_throw_canceled_exception_when_canceled()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            using (var source = new CancellationTokenSource(100))
            {
                Assert.That(async () => await client.GetResponse<Value>(new GetValue {Discard = true}, source.Token), Throws.TypeOf<TaskCanceledException>());
            }
        }

        [Test]
        public async Task Should_throw_canceled_exception_when_already_canceled()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                Assert.That(async () => await client.GetResponse<Value>(new GetValue {Discard = true}, source.Token), Throws.TypeOf<TaskCanceledException>());
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
        public async Task Should_match_the_first_result()
        {
            var client = Bus.CreateRequestClient<RegisterMember>(InputQueueAddress);

            var (registered, existing) = await client.GetResponse<MemberRegistered, ExistingMemberFound>(new RegisterMember());

            await registered;

            Assert.That(async () => await existing, Throws.TypeOf<TaskCanceledException>());
        }

        [Test]
        public async Task Should_match_the_second_result()
        {
            var client = Bus.CreateRequestClient<RegisterMember>(InputQueueAddress);

            var (registered, existing) = await client.GetResponse<MemberRegistered, ExistingMemberFound>(new RegisterMember() {MemberId = "Johnny5"});

            await existing;

            Assert.That(async () => await registered, Throws.TypeOf<TaskCanceledException>());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<RegisterMember>(configurator, context =>
            {
                if (context.Message.MemberId == "Johnny5")
                    return context.RespondAsync<ExistingMemberFound>(new
                    {
                        context.Message.MemberId,
                    });

                return context.RespondAsync<MemberRegistered>(new
                {
                    context.Message.MemberId,
                });
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
        public async Task Should_handle_cancellation()
        {
            var timeout = TimeSpan.FromMilliseconds(100);
            var ctsTimeout = TimeSpan.FromMilliseconds(100);

            int numOfLoops = 5;
            for (var i = 0; i < numOfLoops; ++i)
            {
                Console.WriteLine($"Sending request {i + 1} of {numOfLoops}");

                try
                {
                    using (var timeoutReq = new CancellationTokenSource(ctsTimeout))
                    {
                        var client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, timeout);

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
        public async Task Should_handle_an_earlier_timeout()
        {
            var timeout = TimeSpan.FromMilliseconds(100);
            var ctsTimeout = TimeSpan.FromMilliseconds(140);

            int numOfLoops = 5;
            for (var i = 0; i < numOfLoops; ++i)
            {
                Console.WriteLine($"Sending request {i + 1} of {numOfLoops}");

                try
                {
                    using (var timeoutReq = new CancellationTokenSource(ctsTimeout))
                    {
                        var client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, timeout);

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
        Uri _auditEndpointAddress;
        Task<ConsumeContext<AuditGetValue>> _audited;
        Task<ConsumeContext<ReturnedValue>> _returned;
        Response<Value> _response;

        [Test]
        public async Task Should_not_copy_the_time_to_live_to_sent_messages()
        {
            ConsumeContext<ReturnedValue> consumeContext = await _returned;

            Assert.That(consumeContext.ExpirationTime.HasValue, Is.False);
        }

        [Test]
        public async Task Should_not_copy_the_time_to_live_to_published_messages()
        {
            ConsumeContext<AuditGetValue> consumeContext = await _audited;

            Assert.That(consumeContext.ExpirationTime.HasValue, Is.False);
        }

        [Test]
        public async Task Should_copy_the_time_to_live_to_the_response()
        {
            Assert.That(_response.ExpirationTime.HasValue, Is.True);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

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
            var handler = SubscribeHandler<Fault<GetValue>>();

            var client = Bus.CreateRequestClient<GetValue>(InputQueueAddress);

            try
            {
                await client.GetResponse<Value>(new GetValue {BlowUp = true});

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
}
