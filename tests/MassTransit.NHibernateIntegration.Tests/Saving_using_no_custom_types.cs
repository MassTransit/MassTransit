namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NHibernate;
    using NHibernate.Mapping.ByCode;
    using NHibernate.Mapping.ByCode.Conformist;
    using NUnit.Framework;


    [TestFixture]
    public class Saving_using_no_custom_types
    {
        [Test]
        public async Task Should_have_the_state_machine()
        {
            var correlationId = Guid.NewGuid();

            await RaiseEvent(correlationId, _machine.ExitFrontDoor, new GirlfriendYelling { CorrelationId = correlationId });

            await RaiseEvent(correlationId, _machine.GotHitByCar, new GotHitByACar { CorrelationId = correlationId });

            var instance = await GetStateMachine(correlationId);

            Assert.IsTrue(instance.Screwed);
        }

        SuperShopper _machine;
        ISessionFactory _sessionFactory;

        [OneTimeSetUp]
        public void Setup()
        {
            _machine = new SuperShopper();
            StateMachineStateUserType<SuperShopper>.SaveAsString(_machine);

            _sessionFactory = new SQLiteSessionFactoryProvider(typeof(ShoppingChoreMap))
                .GetSessionFactory();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _sessionFactory.Dispose();
        }

        async Task RaiseEvent<T>(Guid id, Event<T> @event, T data)
            where T : class
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();

            var save = false;
            var instance = session.Get<ShoppingChore>(id, LockMode.Upgrade);
            if (instance == null)
            {
                instance = new ShoppingChore(id);
                save = true;
            }

            await _machine.RaiseEvent(instance, @event, data);

            if (save)
                await session.SaveAsync(instance);
            else
                await session.UpdateAsync(instance);

            await transaction.CommitAsync();
        }

        async Task<ShoppingChore> GetStateMachine(Guid id)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();

            var result = session.QueryOver<ShoppingChore>()
                .Where(x => x.CorrelationId == id)
                .SingleOrDefault<ShoppingChore>();

            await transaction.CommitAsync();

            return result;
        }


        class ShoppingChoreMap :
            ClassMapping<ShoppingChore>
        {
            public ShoppingChoreMap()
            {
                Lazy(false);
                Table("ShoppingChore");

                Id(x => x.CorrelationId, x => x.Generator(Generators.Assigned));

                Property(x => x.CurrentState);
                Property(x => x.Everything);

                Property(x => x.Screwed);
            }
        }


        /// <summary>
        /// Why to exit the door to go shopping
        /// </summary>
        class GirlfriendYelling
        {
            public Guid CorrelationId { get; set; }
        }


        class GotHitByACar
        {
            public Guid CorrelationId { get; set; }
        }


        class ShoppingChore :
            SagaStateMachineInstance
        {
            protected ShoppingChore()
            {
            }

            public ShoppingChore(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public string CurrentState { get; set; }
            public int Everything { get; set; }
            public bool Screwed { get; set; }

            public Guid CorrelationId { get; set; }
        }


        class SuperShopper :
            MassTransitStateMachine<ShoppingChore>
        {
            public SuperShopper()
            {
                InstanceState(x => x.CurrentState);

                CompositeEvent(() => EndOfTheWorld, x => x.Everything, CompositeEventOptions.IncludeInitial, ExitFrontDoor, GotHitByCar);

                Initially(
                    When(ExitFrontDoor)
                        .Then(context => Console.Write("Leaving!"))
                        .TransitionTo(OnTheWayToTheStore));

                During(OnTheWayToTheStore,
                    When(GotHitByCar)
                        .Then(context => Console.WriteLine("Ouch!!"))
                        .Finalize());

                DuringAny(
                    When(EndOfTheWorld)
                        .Then(context => Console.WriteLine("Screwed!!"))
                        .Then(context => context.Instance.Screwed = true));
            }

            public Event<GirlfriendYelling> ExitFrontDoor { get; private set; }
            public Event<GotHitByACar> GotHitByCar { get; private set; }
            public Event EndOfTheWorld { get; private set; }

            public State OnTheWayToTheStore { get; private set; }
        }
    }
}
