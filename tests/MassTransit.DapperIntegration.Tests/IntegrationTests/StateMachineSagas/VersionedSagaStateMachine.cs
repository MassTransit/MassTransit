namespace MassTransit.DapperIntegration.Tests.IntegrationTests.StateMachines
{
    using Common;


    public class VersionedSagaStateMachine : MassTransitStateMachine<VersionedBehaviorSaga>
    {
        public VersionedSagaStateMachine()
        {
            InstanceState(c => c.CurrentState);

            Event(() => OnDelete, e => e.CorrelateBy((s, c) => s.Name == c.Message.Name));

            Initially(
                When(OnCreate)
                    .Then(ctx => ctx.Saga.Name = ctx.Message.Name)
                    .TransitionTo(Ready)
            );

            During(Ready,
                When(OnUpdate)
                    .Then(ctx => ctx.Saga.Name = ctx.Message.Name),
                When(OnDelete)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }

        public State Ready { get; } = null!;

        public Event<CreateSaga> OnCreate { get; } = null!;
        public Event<UpdateSaga> OnUpdate { get; } = null!;
        public Event<DeleteSagaByName> OnDelete { get; } = null!;
    }
}
