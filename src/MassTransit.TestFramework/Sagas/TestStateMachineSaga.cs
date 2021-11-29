namespace MassTransit.TestFramework.Sagas
{
    public class TestStateMachineSaga :
        MassTransitStateMachine<TestInstance>
    {
        public TestStateMachineSaga()
        {
            Event(() => Updated, x => x.CorrelateBy(p => p.Key, m => m.Message.TestKey));

            Initially(
                When(Started)
                    .Then(context => context.Saga.Key = context.Message.TestKey)
                    .Activity(x => x.OfInstanceType<PublishTestStartedActivity>())
                    .TransitionTo(Active));

            During(Active,
                When(Updated)
                    .Publish(context => new TestUpdated
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        TestKey = context.Saga.Key
                    })
                    .TransitionTo(Done)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        public State Active { get; private set; }
        public State Done { get; private set; }

        public Event<StartTest> Started { get; private set; }
        public Event<UpdateTest> Updated { get; private set; }
    }
}
