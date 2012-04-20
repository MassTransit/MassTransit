namespace MassTransit.Tests.Distributor
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using BusConfigurators;
    using Magnum.TestFramework;
    using MassTransit.Distributor.Messages;
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
}
