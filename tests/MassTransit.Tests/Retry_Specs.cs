namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class When_specifying_no_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;
            });

            await fault;

            _attempts.ShouldBe(1);
        }

        int _attempts;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.None());

            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_specifying_the_default_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;

                return Task.CompletedTask;
            });

            await fault;

            _attempts.ShouldBe(1);
        }

        int _attempts;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_specifying_retry_for_the_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;

                return Task.CompletedTask;
            });

            await fault;

            Consumer.Attempts.ShouldBe(6);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new Consumer(), x =>
            {
                x.UseRetry(r => r.Immediate(5));
            });
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public static int Attempts;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                Interlocked.Increment(ref Attempts);

                throw new IntentionalTestException();
            }
        }
    }


    [TestFixture]
    public class When_specifying_the_bus_level_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_twice()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;
            });

            await fault;

            _attempts.ShouldBe(2);
        }

        int _attempts;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_specifying_the_bus_level_retry_policy_for_base_type :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_twice()
        {
            Task<ConsumeContext<Fault<BaseMessage>>> fault = SubscribeHandler<Fault<BaseMessage>>();

            await InputQueueSendEndpoint.Send(new BaseMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;
            });

            await fault;

            _attempts.ShouldBe(2);
        }

        int _attempts;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<BaseMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }


        public interface IBaseMessage
        {
        }


        public class BaseMessage :
            IBaseMessage
        {
        }
    }


    [TestFixture]
    public class When_both_levels_of_retry_are_specified :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_inner_policy()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));

            await fault;

            _attempts.ShouldBe(4);

            _lastCount.ShouldBe(2);
            _lastAttempt.ShouldBe(3);
        }

        int _attempts;
        int _lastAttempt;
        int _lastCount;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(3));
            Handler<PingMessage>(configurator, async context =>
            {
                Interlocked.Increment(ref _attempts);

                _lastAttempt = context.GetRetryAttempt();
                _lastCount = context.GetRetryCount();

                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_multiple_retry_policies_are_specified :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_inner_policy()
        {
            Task<ConsumeContext<Fault<YourMessage>>> fault = SubscribeHandler<Fault<YourMessage>>();

            await InputQueueSendEndpoint.Send(new YourMessage { Text = "Hi" }, Pipe.Execute<SendContext<YourMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));

            await fault;

            _attempts.ShouldBe(6);

            _lastCount.ShouldBe(4);
            _lastAttempt.ShouldBe(5);
        }

        static int _attempts;
        static int _lastAttempt;
        static int _lastCount;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageRetry(retry =>
            {
                retry.Immediate(5);
                retry.Ignore<TimeoutException>();
            });

            configurator.UseMessageRetry(retry =>
            {
                retry.Handle<TimeoutException>();
                retry.Interval(2, TimeSpan.FromSeconds(1));
            });

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new YourMessageConsumer());
        }


        public class YourMessage
        {
            public string Text { get; set; }
        }


        class YourMessageConsumer :
            IConsumer<YourMessage>,
            IConsumer<Fault<YourMessage>>
        {
            public Task Consume(ConsumeContext<YourMessage> context)
            {
                Interlocked.Increment(ref _attempts);

                _lastAttempt = context.GetRetryAttempt();
                _lastCount = context.GetRetryCount();

                TestContext.Out.WriteLine($"Attempt: {context.GetRetryAttempt()}");

                throw new Exception("Big bad exception");
            }

            public Task Consume(ConsumeContext<Fault<YourMessage>> context)
            {
                var faultRetryCount = context.Headers.Get(MessageHeaders.FaultRetryCount, default(int?)) ?? 0;

                TestContext.Out.WriteLine($"Attempt (from fault consumer): {faultRetryCount}");

                TestContext.Out.WriteLine(@"Faulted, won't retry any more.");

                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class When_the_retry_is_specified_within_the_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_proper_counts()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));

            await fault;

            Consumer.Attempts.ShouldBe(4);

            Consumer.LastCount.ShouldBe(2);
            Consumer.LastAttempt.ShouldBe(3);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumer>(cfg =>
            {
                cfg.UseRetry(x => x.Immediate(3));
            });
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public static int Attempts;

            public static int LastAttempt;

            public static int LastCount;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                Interlocked.Increment(ref Attempts);

                LastAttempt = context.GetRetryAttempt();
                LastCount = context.GetRetryCount();

                throw new IntentionalTestException();
            }
        }
    }


    [TestFixture]
    public class When_ignoring_the_exception :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_just_call_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));

            await fault;

            _attempts.ShouldBe(1);

            _lastAttempt.ShouldBe(0);
            _lastCount.ShouldBe(0);
        }

        int _attempts;
        int _lastAttempt;
        int _lastCount;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x =>
            {
                x.Ignore<IntentionalTestException>();
                x.Immediate(1);
            });

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                Interlocked.Increment(ref _attempts);

                _lastAttempt = context.GetRetryAttempt();
                _lastCount = context.GetRetryCount();

                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_ignoring_the_inner_exception :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_just_call_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));

            await fault;

            _attempts.ShouldBe(1);

            _lastAttempt.ShouldBe(0);
        }

        int _attempts;
        int _lastAttempt;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x =>
            {
                x.Ignore<IntentionalTestException>();
                x.Immediate(1);
            });

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                Interlocked.Increment(ref _attempts);

                _lastAttempt = context.GetRetryAttempt();

                throw new Exception("Yonder", new IntentionalTestException());
            });
        }
    }


    [TestFixture]
    public class When_eventually_you_succeed :
        InMemoryTestFixture
    {
        [Test]
        public async Task After_try_trying_again()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _completed.Task;

            await Task.Delay(100);

            _attempts.ShouldBe(2);
        }

        int _attempts;
        TaskCompletionSource<PingMessage> _completed;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<PingMessage>();

            Handler<PingMessage>(configurator, context =>
            {
                var attempt = Interlocked.Increment(ref _attempts);

                if (attempt == 1)
                    throw new IntentionalTestException();

                _completed.TrySetResult(context.Message);

                return Task.CompletedTask;
            });
        }
    }


    [TestFixture]
    public class When_observing_retries :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_call_the_observer()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _completed.Task;

            await _observed.Task;

            Assert.That(_attempts, Is.EqualTo(4));

            var payload = await _payload.Task;

            Assert.That(payload.PostCreateCount, Is.EqualTo(0), "PostCreateCount");
            Assert.That(payload.RetryCompletedCount, Is.EqualTo(1), "RetryCompletedCount");
            Assert.That(payload.PreRetryCount, Is.EqualTo(1), "PreRetryCount");
            Assert.That(payload.PostFaultCount, Is.EqualTo(1), "PostFaultCount");
            Assert.That(payload.RetryFaultCount, Is.EqualTo(0), "RetryFaultCount");
        }

        int _attempts;
        TaskCompletionSource<PingMessage> _completed;
        TaskCompletionSource<RetryContext> _observed;
        TaskCompletionSource<RetryPayload> _payload;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _observed = GetTask<RetryContext>();
            _payload = GetTask<RetryPayload>();
            configurator.UseRetry(x =>
            {
                x.Immediate(5);
                x.ConnectRetryObserver(new RetryObserver(_observed, _payload));
            });

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<PingMessage>();

            Handler<PingMessage>(configurator, context =>
            {
                var attempt = Interlocked.Increment(ref _attempts);

                if (attempt <= 3)
                    throw new IntentionalTestException();

                _completed.TrySetResult(context.Message);

                return Task.CompletedTask;
            });
        }


        class RetryPayload
        {
            public int PostCreateCount;
            public int PostFaultCount;
            public int PreRetryCount;
            public int RetryCompletedCount;
            public int RetryFaultCount;
        }


        class RetryObserver :
            IRetryObserver
        {
            readonly TaskCompletionSource<RetryContext> _completionSource;
            readonly TaskCompletionSource<RetryPayload> _payload;

            public RetryObserver(TaskCompletionSource<RetryContext> completionSource, TaskCompletionSource<RetryPayload> payload)
            {
                _completionSource = completionSource;
                _payload = payload;
            }

            public Task PostCreate<T>(RetryPolicyContext<T> context)
                where T : class, PipeContext
            {
                var payload = context.Context.GetOrAddPayload(() => new RetryPayload());
                payload.PostCreateCount++;

                return Task.CompletedTask;
            }

            public Task PostFault<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                var payload = context.Context.GetOrAddPayload(() => new RetryPayload());
                payload.PostFaultCount++;

                return Task.CompletedTask;
            }

            public Task PreRetry<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                var payload = context.Context.GetOrAddPayload(() => new RetryPayload());
                payload.PreRetryCount++;

                return Task.CompletedTask;
            }

            public Task RetryFault<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                var payload = context.Context.GetOrAddPayload(() => new RetryPayload());
                payload.RetryFaultCount++;

                return Task.CompletedTask;
            }

            public Task RetryComplete<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                _completionSource.TrySetResult(context);

                if (context.Context.TryGetPayload(out RetryPayload payload))
                {
                    payload.RetryCompletedCount++;
                    _payload.TrySetResult(payload);
                }
                else
                    _payload.TrySetException(new PayloadNotFoundException());

                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class When_you_say_deuces_and_stop_the_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_cancel_the_retry_and_give_it_up()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _retryObserver.Completed;

            await Task.Delay(100);

            _attempts.ShouldBe(1);
        }

        int _attempts;
        TaskCompletionSource<PingMessage> _completed;
        RetryObserver _retryObserver;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _retryObserver = new RetryObserver();

            configurator.UseRetry(x =>
            {
                x.Interval(1, TimeSpan.FromMinutes(1));
                x.ConnectRetryObserver(_retryObserver);
            });

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<PingMessage>();

            Handler<PingMessage>(configurator, context =>
            {
                var attempt = Interlocked.Increment(ref _attempts);
                if (attempt == 1)
                    throw new IntentionalTestException();

                _completed.TrySetResult(context.Message);

                return Task.CompletedTask;
            });
        }


        class RetryObserver :
            IRetryObserver
        {
            readonly TaskCompletionSource<RetryContext> _completionSource;

            public RetryObserver()
            {
                _completionSource = TaskUtil.GetTask<RetryContext>();
            }

            public Task<RetryContext> Completed => _completionSource.Task;

            public Task PostCreate<T>(RetryPolicyContext<T> context)
                where T : class, PipeContext
            {
                return Task.CompletedTask;
            }

            public Task PostFault<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                _completionSource.TrySetResult(context);

                return Task.CompletedTask;
            }

            public Task PreRetry<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                return Task.CompletedTask;
            }

            public Task RetryFault<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                return Task.CompletedTask;
            }

            public Task RetryComplete<T>(RetryContext<T> context)
                where T : class, PipeContext
            {
                return Task.CompletedTask;
            }
        }
    }
}
