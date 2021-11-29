namespace MassTransit.Tests.Middleware.Agents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Agents;
    using MassTransit.Internals;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Creating_a_base_agent
    {
        [Test]
        public async Task Should_fault_on_ready_faulted()
        {
            for (var i = 0; i < 50; i++)
            {
                var supervisor = new Supervisor();

                var provocateur = new Agent();
                provocateur.SetNotReady(new IntentionalTestException("So not ready."));
                supervisor.Add(provocateur);

                supervisor.SetReady();

                Assert.That(async () => await supervisor.Ready.OrTimeout(s: 5), Throws.TypeOf<AggregateException>());

                await supervisor.Stop().OrTimeout(s: 5);

                await supervisor.Completed.OrTimeout(s: 5);
            }
        }

        [Test]
        public async Task Should_simply_stop()
        {
            var supervisor = new Supervisor();

            supervisor.SetReady();

            await supervisor.Ready.OrTimeout(s: 5);

            await supervisor.Stop();

            await supervisor.Completed.OrTimeout(s: 5);
        }

        [Test]
        public async Task Should_stop_and_complete_with_a_chain_of_command()
        {
            var supervisor = new Supervisor();

            var manager = new Supervisor();
            supervisor.Add(manager);

            var provocateur = new Agent();
            manager.Add(provocateur);

            manager.SetReady();
            supervisor.SetReady();
            provocateur.SetReady();

            Console.WriteLine("Waiting for Ready...");

            await supervisor.Ready.OrTimeout(s: 5);

            Console.WriteLine("Stopping");

            await supervisor.Stop();

            Console.WriteLine("Waiting for Completed...");

            await supervisor.Completed.OrTimeout(s: 5);
        }

        [Test]
        public async Task Should_stop_and_complete_with_an_agent()
        {
            var supervisor = new Supervisor();

            var provocateur = new Agent();

            provocateur.SetReady();
            supervisor.SetReady();

            supervisor.Add(provocateur);

            Console.WriteLine("Waiting for Ready...");

            await supervisor.Ready.OrTimeout(s: 5);

            Console.WriteLine("Stopping");

            await supervisor.Stop().OrTimeout(s: 5);

            Console.WriteLine("Waiting for Completed...");

            await supervisor.Completed.OrTimeout(s: 5);
        }
    }


    [TestFixture]
    public class Caching_a_pipe_context
    {
        [Test]
        public async Task Should_allow_active_instances()
        {
            ISupervisor<SimpleContext> supervisor = new PipeContextSupervisor<SimpleContext>(new SimpleContextFactory());

            var count = 0;
            var lastValue = string.Empty;

            IPipe<SimpleContext> pipe = Pipe.New<SimpleContext>(x => x.UseExecute(context =>
            {
                if (Interlocked.Increment(ref count) % 2 == 0)
                    throw new IntentionalTestException("It's odd that we throw when it's even.");

                lastValue = context.Value;
            }));

            await supervisor.Send(pipe);
            Assert.That(async () => await supervisor.Send(pipe), Throws.TypeOf<IntentionalTestException>());
            await supervisor.Send(pipe);

            Assert.That(lastValue, Is.EqualTo("2"));
            Assert.That(count, Is.EqualTo(3));

            await supervisor.Stop();

            await supervisor.Completed.OrTimeout(s: 5);
        }

        [Test]
        public async Task Should_support_disconnection()
        {
            ISupervisor<SimpleContext> supervisor = new PipeContextSupervisor<SimpleContext>(new SimpleContextFactory());

            var count = 0;
            var lastValue = string.Empty;

            IPipe<SimpleContext> pipe = Pipe.New<SimpleContext>(x => x.UseExecute(context =>
            {
                if (Interlocked.Increment(ref count) % 2 == 0)
                    context.Invalidate();

                lastValue = context.Value;
            }));

            await supervisor.Send(pipe);
            await supervisor.Send(pipe);
            await supervisor.Send(pipe);

            Assert.That(lastValue, Is.EqualTo("2"));
            Assert.That(count, Is.EqualTo(3));

            await supervisor.Stop();

            await supervisor.Completed.OrTimeout(s: 5);
        }


        interface SimpleContext :
            PipeContext
        {
            string Value { get; }

            void Invalidate();
        }


        class SimpleContextImpl :
            BasePipeContext,
            SimpleContext,
            IAsyncDisposable
        {
            async ValueTask IAsyncDisposable.DisposeAsync()
            {
                await Console.Out.WriteLineAsync($"Disposing {Value}");
            }

            public string Value { get; set; }

            public void Invalidate()
            {
                OnInvalid?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler OnInvalid;
        }


        class ActiveSimpleContext :
            SimpleContext
        {
            readonly SimpleContext _context;

            public ActiveSimpleContext(SimpleContext context, CancellationToken cancellationToken)
            {
                CancellationToken = cancellationToken;
                _context = context;
            }

            public CancellationToken CancellationToken { get; }

            bool PipeContext.HasPayloadType(Type payloadType)
            {
                return _context.HasPayloadType(payloadType);
            }

            bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
            {
                return _context.TryGetPayload(out payload);
            }

            TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            {
                return _context.GetOrAddPayload(payloadFactory);
            }

            T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            {
                return _context.AddOrUpdatePayload(addFactory, updateFactory);
            }

            string SimpleContext.Value => _context.Value;

            void SimpleContext.Invalidate()
            {
                _context.Invalidate();
            }
        }


        class SimpleContextFactory :
            IPipeContextFactory<SimpleContext>
        {
            long _id;

            public IPipeContextAgent<SimpleContext> CreateContext(ISupervisor supervisor)
            {
                var context = new SimpleContextImpl
                {
                    Value = Interlocked.Increment(ref _id).ToString()
                };

                IPipeContextAgent<SimpleContext> contextHandle = supervisor.AddContext<SimpleContext>(context);

                void SimpleContextOnInvalid(object sender, EventArgs args)
                {
                    contextHandle.DisposeAsync();
                }

                context.OnInvalid += SimpleContextOnInvalid;

                return contextHandle;
            }

            public IActivePipeContextAgent<SimpleContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<SimpleContext> context,
                CancellationToken cancellationToken = default)
            {
                return supervisor.AddActiveContext(context, CreateActiveContext(context.Context, cancellationToken));
            }

            async Task<SimpleContext> CreateActiveContext(Task<SimpleContext> context, CancellationToken cancellationToken)
            {
                var existingContext = await context.ConfigureAwait(false);

                return new ActiveSimpleContext(existingContext, cancellationToken);
            }
        }
    }
}
