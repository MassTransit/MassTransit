namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga
{
    using System.Threading.Tasks;
    using DataAccess;
    using Events;


    public class SlowConcurrentSagaStateMachine : MassTransitStateMachine<SlowConcurrentSaga>
    {
        public SlowConcurrentSagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => Begin, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => IncrementCounterSlowly, x => x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(Begin)
                    .Then(context => context.Instance.Counter = 1)
                    .TransitionTo(Started));

            During(Started,
                When(IncrementCounterSlowly)
                    .ThenAsync(async context =>
                    {
                        await Task.Delay(5000);
                        context.Instance.Counter++;
                    })
                    .TransitionTo(DidIncrement));
        }

        public Event<Begin> Begin { get; set; }

        public Event<IncrementCounterSlowly> IncrementCounterSlowly { get; set; }

        public State Started { get; set; }

        public State DidIncrement { get; set; }
    }
}
