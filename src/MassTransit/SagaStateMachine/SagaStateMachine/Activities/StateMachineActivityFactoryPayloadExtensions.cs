namespace MassTransit.SagaStateMachine
{
    public static class StateMachineActivityFactoryPayloadExtensions
    {
        public static IStateMachineActivityFactory GetStateMachineActivityFactory(this PipeContext context)
        {
            return context.TryGetPayload<IStateMachineActivityFactory>(out var factory)
                ? factory
                : new DefaultConstructorStateMachineActivityFactory();
        }
    }
}
