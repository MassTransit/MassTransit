namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_using_retry_in_a_state_machine
    {
        [Test]
        public async Task Should_retry_the_activities()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            Assert.That(async () => await machine.RaiseEvent(instance, machine.Initialized), Throws.TypeOf<IntentionalTestException>());

            Assert.That(instance.Caught, Is.False);
        }

        [Test]
        public async Task Should_retry_the_activities_with_message()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            Assert.That(async () => await machine.RaiseEvent(instance, machine.InitializedA, new A()), Throws.TypeOf<IntentionalTestException>());

            Assert.That(instance.Caught, Is.False);
        }

        [Test]
        public async Task Should_retry_the_activities_and_still_allow_catch()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            await machine.RaiseEvent(instance, machine.InitializedWithCatch);

            Assert.That(instance.Caught, Is.True);
        }

        [Test]
        public async Task Should_retry_the_activities_and_still_allow_catch_with_message()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            await machine.RaiseEvent(instance, machine.InitializedB, new B());

            Assert.That(instance.Caught, Is.True);
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public int AttemptCount { get; set; }
            public bool Caught { get; set; }

            public Guid CorrelationId { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .Retry(r => r.Intervals(10, 10, 10), x => x
                            .Then(context =>
                            {
                                context.Saga.AttemptCount++;
                                throw new IntentionalTestException("Time to crash");
                            })),
                    When(InitializedWithCatch)
                        .Retry(r => r.Intervals(10, 10, 10), x => x
                            .Then(context =>
                            {
                                context.Saga.AttemptCount++;
                                throw new IntentionalTestException("Time to crash");
                            }))
                        .Catch<IntentionalTestException>(x => x.Then(context => context.Saga.Caught = true)),
                    When(InitializedA)
                        .Retry(r => r.Intervals(10, 10, 10), x => x
                            .Then(context =>
                            {
                                context.Saga.AttemptCount++;
                                throw new IntentionalTestException("Time to crash");
                            })),
                    When(InitializedB)
                        .Retry(r => r.Intervals(10, 10, 10), x => x
                            .Then(context =>
                            {
                                context.Saga.AttemptCount++;
                                throw new IntentionalTestException("Time to crash");
                            }))
                        .Catch<IntentionalTestException>(x => x.Then(context => context.Saga.Caught = true))
                );
            }

            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
            public Event InitializedWithCatch { get; private set; }

            public Event<A> InitializedA { get; private set; }
            public Event<B> InitializedB { get; private set; }
        }


        class A
        {
        }


        class B
        {
        }
    }
}
