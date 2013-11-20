namespace MassTransit.Tests.Distributor
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Distributor.Messages;
    using MassTransit.Pipeline.Inspectors;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class Using_a_distributor_saga :
        LoopbackLocalAndRemoteTestFixture
    {
        InMemorySagaRepository<MySaga> _sagaRepository;

        [Test]
        public void Should_have_the_subscription_for_the_initiating()
        {
            LocalBus.HasSubscription<A>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_orchestrating()
        {
            LocalBus.HasSubscription<B>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_observed()
        {
            LocalBus.HasSubscription<C>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_A()
        {
            LocalBus.HasSubscription<WorkerAvailable<A>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_B()
        {
            LocalBus.HasSubscription<WorkerAvailable<B>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_C()
        {
            LocalBus.HasSubscription<WorkerAvailable<C>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        public Using_a_distributor_saga()
        {
            _sagaRepository = new InMemorySagaRepository<MySaga>();
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Distributor(x => x.Saga<MySaga>(_sagaRepository));
        }

        class MySaga :
            InitiatedBy<A>,
            Orchestrates<B>,
            Observes<C, MySaga>,
            ISaga
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            public MySaga(Guid correlationId)
                : this()
            {
                
            }
            public MySaga()
            {
                BusinessId = "Hello";
            }

            public Guid CorrelationId { get; set; }
            public string BusinessId { get; set; }
            public IServiceBus Bus { get; set; }

            public void Consume(A message)
            {
                FutureA.Set(message);
            }

            public void Consume(B message)
            {
                FutureB.Set(message);
            }

            public void Consume(C message)
            {
                FutureC.Set(message);
            }

            public Expression<Func<MySaga, C, bool>> GetBindExpression()
            {
                return (saga, message) => saga.BusinessId == BusinessId;
            }
        }

        class A :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class B:
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class C
        {
            public string BusinessId { get; set; }
        }
    }

    [TestFixture, Explicit]
    public class Using_a_distributor_and_worker_saga :
        LoopbackLocalAndRemoteTestFixture
    {
        InMemorySagaRepository<MySaga> _sagaRepository;
        TimeSpan _testTimeout = Debugger.IsAttached ? 5.Minutes() : 30.Seconds();

        [Test]
        public void Should_deliver_a_published_message()
        {
            Publish<A>();
            Publish<C>();
            Publish<B>();

            MySaga.FutureA.IsAvailable(_testTimeout).ShouldBeTrue("Missing regular consumer");
            MySaga.FutureC.IsAvailable(_testTimeout).ShouldBeTrue("Missing context consumer");
            MySaga.FutureB.IsAvailable(_testTimeout).ShouldBeTrue("Missing selective consumer");
        }

        void Publish<T>()
            where T : class, new()
        {
            LocalBus.HasSubscription<T>().Any()
                .ShouldBeTrue("Message subscription was not found");
            LocalBus.HasSubscription<WorkerAvailable<T>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
            RemoteBus.HasSubscription<Distributed<T>>().Any()
                .ShouldBeTrue("Message subscription was not found");

            var message = new T();
            LocalBus.Endpoint.Send(message, context => { context.SendResponseTo(LocalBus); });
        }

        public Using_a_distributor_and_worker_saga()
        {
            _sagaRepository = new InMemorySagaRepository<MySaga>();
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Distributor(x => x.Saga<MySaga>(_sagaRepository));
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Worker(x => x.Saga<MySaga>(_sagaRepository));
        }

        class MySaga :
            InitiatedBy<A>,
            Orchestrates<B>,
            Observes<C, MySaga>,
            ISaga
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            public MySaga(Guid correlationId)
                : this()
            {
                
            }
            public MySaga()
            {
                BusinessId = "Hello";
            }

            public Guid CorrelationId { get; set; }
            public string BusinessId { get; set; }
            public IServiceBus Bus { get; set; }

            public void Consume(A message)
            {
                FutureA.Set(message);
            }

            public void Consume(B message)
            {
                FutureB.Set(message);
            }

            public void Consume(C message)
            {
                FutureC.Set(message);
            }

            public Expression<Func<MySaga, C, bool>> GetBindExpression()
            {
                return (saga, message) => saga.BusinessId == BusinessId;
            }
        }

        class A :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class B:
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class C
        {
            public string BusinessId { get; set; }
        }
    }

    [TestFixture]
    public class Using_a_worker_saga :
        LoopbackLocalAndRemoteTestFixture
    {
        InMemorySagaRepository<MySaga> _sagaRepository;

        [Test]
        public void Should_have_the_subscription_for_the_initiating()
        {
            LocalBus.HasSubscription<Distributed<A>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_orchestrating()
        {
            LocalBus.HasSubscription<Distributed<B>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_observed()
        {
            LocalBus.HasSubscription<Distributed<C>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_ping_worker()
        {
            RemoteBus.HasSubscription<PingWorker>().Any()
                .ShouldBeTrue("PingWorker subscription was not found.");
        }

        public Using_a_worker_saga()
        {
            _sagaRepository = new InMemorySagaRepository<MySaga>();
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Worker(x => x.Saga<MySaga>(_sagaRepository));
        }

        class MySaga :
            InitiatedBy<A>,
            Orchestrates<B>,
            Observes<C, MySaga>,
            ISaga
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            public MySaga(Guid correlationId)
                : this()
            {
                
            }
            public MySaga()
            {
                BusinessId = "Hello";
            }

            public Guid CorrelationId { get; set; }
            public string BusinessId { get; set; }
            public IServiceBus Bus { get; set; }

            public void Consume(A message)
            {
                FutureA.Set(message);
            }

            public void Consume(B message)
            {
                FutureB.Set(message);
            }

            public void Consume(C message)
            {
                FutureC.Set(message);
            }

            public Expression<Func<MySaga, C, bool>> GetBindExpression()
            {
                return (saga, message) => saga.BusinessId == BusinessId;
            }
        }

        class A :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class B:
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class C
        {
            public string BusinessId { get; set; }
        }
    }
}
