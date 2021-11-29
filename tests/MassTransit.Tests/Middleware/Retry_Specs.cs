namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit.Middleware;
    using NUnit.Framework;
    using RetryPolicies;
    using Util;


    [TestFixture]
    public class Using_the_retry_filter
    {
        [Test]
        public async Task Should_be_observable_at_each_retry()
        {
            var observer = new RetryObserver();

            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.None());
                x.UseRetry(r =>
                {
                    r.Interval(4, TimeSpan.FromMilliseconds(2));
                    r.ConnectRetryObserver(observer);
                });
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(5));

            await observer.PostFault;

            Assert.That(observer.PostFaultCount, Is.EqualTo(4));
            Assert.That(observer.RetryFaultCount, Is.EqualTo(1));

            var retryFault = await observer.RetryFault;

            Assert.That(retryFault.RetryCount, Is.EqualTo(4));
        }

        [Test]
        public async Task Should_be_observable_at_the_inner_level()
        {
            var observer = new RetryObserver();

            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.None());
                x.UseRetry(r =>
                {
                    r.Interval(4, TimeSpan.FromMilliseconds(2));
                    r.ConnectRetryObserver(observer);
                });
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(5));

            await observer.RetryFault;

            Assert.That(observer.RetryFaultCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Should_be_observable_at_the_outer_level()
        {
            var observer = new RetryObserver();

            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.None();
                    r.ConnectRetryObserver(observer);
                });
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(5));

            await observer.RetryFault;

            Assert.That(observer.RetryFaultCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Should_be_observable_with_no_retry_allowed()
        {
            var observer = new RetryObserver();

            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.None();
                    r.ConnectRetryObserver(observer);
                });
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(1));

            await observer.RetryFault;

            Assert.That(observer.RetryFaultCount, Is.EqualTo(1));
        }

        [Test]
        public void Should_calculate_the_retry_intervals_once()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.Exponential(10, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(30)));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(11));
        }

        [Test]
        public void Should_clean_memory_after_retry()
        {
            var disposed = 0;

            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.SetRetryPolicy(filter => new TestRetryPolicy(1, () => disposed++)));
                x.UseExecute(payload =>
                {
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(disposed, Is.EqualTo(1));
        }

        [Test]
        public void Should_ignore_the_exception()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.Ignore<IntentionalTestException>();
                    r.Immediate(4);
                });
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Should_ignore_the_inner_exception()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.Ignore<IntentionalTestException>();
                    r.Immediate(4);
                });
                x.UseExecute(payload =>
                {
                    count++;
                    throw new Exception("Inside is death", new IntentionalTestException("Kaboom!"));
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<Exception>());

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Should_not_stack_overflow_with_a_ridiculous_retry_count()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.Interval(20000, TimeSpan.Zero));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(20001));
        }

        [Test]
        public async Task Should_retry_and_then_succeed_without_repeating_forever()
        {
            var observer = new RetryObserver();

            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.Immediate(1);
                    r.ConnectRetryObserver(observer);
                });
                x.UseExecute(payload =>
                {
                    var current = Interlocked.Increment(ref count);

                    if (current == 1)
                        throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            await pipe.Send(context);

            Assert.That(count, Is.EqualTo(2));

            Assert.That(observer.PostFault.IsCompleted);
            Assert.That(observer.RetryComplete.IsCompleted);
        }

        [Test]
        public void Should_retry_the_specified_times_and_fail()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public void Should_support_overloading_downstream()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseRetry(r => r.None());
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Should_support_overloading_downstream_either_way()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.None());
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public void Should_support_overloading_downstream_with_dispatch()
        {
            var count = 0;
            IPipe<CommandContext> pipe = Pipe.New<CommandContext>(x =>
            {
                x.UseRetry(r => r.None());
                x.UseDispatch(new PipeContextConverterFactory(), d =>
                {
                    d.Pipe<CommandContext<SetConcurrencyLimit>>(p =>
                    {
                        p.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                        p.UseExecute(payload =>
                        {
                            count++;
                            throw new IntentionalTestException("Kaboom!");
                        });
                    });
                });
            });

            Assert.That(async () => await pipe.SetConcurrencyLimit(32), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public void Should_support_overloading_downstream_with_dispatch_either_that()
        {
            var count = 0;
            IPipe<CommandContext> pipe = Pipe.New<CommandContext>(x =>
            {
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseRetry(r => r.None());
                x.UseDispatch(new PipeContextConverterFactory(), d =>
                {
                    d.Pipe<CommandContext<SetConcurrencyLimit>>(p =>
                    {
                        p.UseExecute(payload =>
                        {
                            count++;
                            throw new IntentionalTestException("Kaboom!");
                        });
                    });
                });
            });

            Assert.That(async () => await pipe.SetConcurrencyLimit(32), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Should_support_overloading_downstream_with_dispatch_either_that_thang()
        {
            var count = 0;
            IPipe<CommandContext> pipe = Pipe.New<CommandContext>(x =>
            {
                x.UseRetry(r => r.SetRetryPolicy(filter => new CommandContextRetryPolicy(new IntervalRetryPolicy(filter, 4, 4, 4, 4))));
                x.UseRetry(r => r.SetRetryPolicy(filter => new CommandContextRetryPolicy(new NoRetryPolicy(filter))));
                x.UseDispatch(new PipeContextConverterFactory(), d =>
                {
                    d.Pipe<CommandContext<SetConcurrencyLimit>>(p =>
                    {
                        p.UseExecute(payload =>
                        {
                            count++;
                            throw new IntentionalTestException("Kaboom!");
                        });
                    });
                });
            });

            Assert.That(async () => await pipe.SetConcurrencyLimit(32), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Should_support_overloading_downstream_with_dispatch_either_way()
        {
            var count = 0;
            IPipe<CommandContext> pipe = Pipe.New<CommandContext>(x =>
            {
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseDispatch(new PipeContextConverterFactory(), d =>
                {
                    d.Pipe<CommandContext<SetConcurrencyLimit>>(p =>
                    {
                        p.UseRetry(r => r.None());
                        p.UseExecute(payload =>
                        {
                            count++;
                            throw new IntentionalTestException("Kaboom!");
                        });
                    });
                });
            });

            Assert.That(async () => await pipe.SetConcurrencyLimit(32), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(1));
        }


        class TestContext :
            BasePipeContext,
            PipeContext
        {
        }


        class RetryObserver :
            IRetryObserver
        {
            readonly TaskCompletionSource<bool> _postFault;
            readonly TaskCompletionSource<RetryContext> _retryComplete;
            readonly TaskCompletionSource<RetryContext> _retryFault;
            int _postFaultCount;
            int _retryFaultCount;

            public RetryObserver()
            {
                _retryFault = TaskUtil.GetTask<RetryContext>();
                _retryComplete = TaskUtil.GetTask<RetryContext>();
                _postFault = TaskUtil.GetTask();
            }

            public int RetryFaultCount => _retryFaultCount;

            public Task<RetryContext> RetryFault => _retryFault.Task;
            public Task<RetryContext> RetryComplete => _retryComplete.Task;

            public int PostFaultCount => _postFaultCount;

            public Task PostFault => _postFault.Task;

            Task IRetryObserver.PostCreate<T>(RetryPolicyContext<T> context)
            {
                return Task.CompletedTask;
            }

            Task IRetryObserver.PostFault<T>(RetryContext<T> context)
            {
                Interlocked.Increment(ref _postFaultCount);

                _postFault.SetCompleted();

                return Task.CompletedTask;
            }

            Task IRetryObserver.PreRetry<T>(RetryContext<T> context)
            {
                return Task.CompletedTask;
            }

            Task IRetryObserver.RetryFault<T>(RetryContext<T> context)
            {
                Interlocked.Increment(ref _retryFaultCount);

                _retryFault.TrySetResult(context);

                return Task.CompletedTask;
            }

            Task IRetryObserver.RetryComplete<T>(RetryContext<T> context)
            {
                _retryComplete.TrySetResult(context);

                return Task.CompletedTask;
            }
        }


        class TestRetryPolicy :
            IRetryPolicy
        {
            readonly Action _onRetryContextDisposed;
            readonly int _retryLimit;

            public TestRetryPolicy(int retryLimit, Action onRetryContextDisposed)
            {
                _retryLimit = retryLimit;
                _onRetryContextDisposed = onRetryContextDisposed;
            }

            public int RetryLimit => _retryLimit;

            void IProbeSite.Probe(ProbeContext context)
            {
                context.Set(new
                {
                    Policy = "test-retry",
                    Limit = _retryLimit
                });
            }

            RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
            {
                return new TestRetryPolicyContext<T>(this, context, _onRetryContextDisposed);
            }

            public bool IsHandled(Exception exception)
            {
                return true;
            }
        }


        class TestRetryPolicyContext<TContext> : RetryPolicyContext<TContext>
            where TContext : class, PipeContext
        {
            readonly CancellationTokenSource _cancellationTokenSource;
            readonly Action _onRetryContextDisposed;
            readonly TestRetryPolicy _policy;

            public TestRetryPolicyContext(TestRetryPolicy policy, TContext context, Action onRetryContextDisposed)
            {
                _policy = policy;
                Context = context;
                _onRetryContextDisposed = onRetryContextDisposed;
                _cancellationTokenSource = new CancellationTokenSource();
            }

            CancellationToken CancellationToken => _cancellationTokenSource.Token;

            public TContext Context { get; }

            public bool CanRetry(Exception exception, out RetryContext<TContext> retryContext)
            {
                retryContext = new TestRetryContext<TContext>(_policy, Context, exception, 0, CancellationToken);

                return _policy.IsHandled(exception);
            }

            Task RetryPolicyContext<TContext>.RetryFaulted(Exception exception)
            {
                return Task.CompletedTask;
            }

            public void Cancel()
            {
                // do nothing
            }

            void IDisposable.Dispose()
            {
                _onRetryContextDisposed();
            }
        }


        class TestRetryContext<TContext> :
            BaseRetryContext<TContext>,
            RetryContext<TContext>
            where TContext : class, PipeContext
        {
            readonly TestRetryPolicy _policy;

            public TestRetryContext(TestRetryPolicy policy, TContext context, Exception exception, int retryCount, CancellationToken cancellationToken)
                : base(context, exception, retryCount, cancellationToken)
            {
                _policy = policy;
            }

            bool RetryContext<TContext>.CanRetry(Exception exception, out RetryContext<TContext> retryContext)
            {
                retryContext = new TestRetryContext<TContext>(_policy, Context, Exception, RetryCount + 1, CancellationToken);

                return RetryAttempt < _policy.RetryLimit;
            }
        }
    }
}
