namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga
{
    using System.Threading.Tasks;
    using Automatonymous;
    using DataAccess;
    using Events;


    public class DeadlockSagaStateMachine : MassTransitStateMachine<DeadlockSaga>
    {
        public DeadlockSagaStateMachine()
        {
            this.InstanceState(x => x.CurrentState);

            this.Event(() => this.Begin, x => x.CorrelateById(context => context.Message.CorrelationId));
            this.Event(() => this.IncrementCounterSlowly, x => x.CorrelateById(context => context.Message.CorrelationId));

            this.Initially(
                this.When(this.Begin)
                    .Then(context => context.Instance.Counter = 1)
                    .TransitionTo(this.Started));

            this.During(this.Started,
                this.When(this.IncrementCounterSlowly)
                    .ThenAsync(async context =>
                    {
                        await Task.Delay(5000);
                        context.Instance.Counter++;
                    })
                    .TransitionTo(this.DidIncrement));
        }
        public Event<Begin> Begin { get; set; }

        public Event<IncrementCounterSlowly> IncrementCounterSlowly { get; set; }

        public State Started { get; set; }

        public State DidIncrement { get; set; }
    }
}
