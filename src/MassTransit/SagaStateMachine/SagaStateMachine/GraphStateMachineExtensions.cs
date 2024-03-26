namespace MassTransit.SagaStateMachine
{
    public static class GraphStateMachineExtensions
    {
        public static StateMachineGraph GetGraph<TSaga>(this StateMachine<TSaga> machine)
            where TSaga : class, SagaStateMachineInstance
        {
            var inspector = new GraphStateMachineVisitor<TSaga>(machine);

            machine.Accept(inspector);

            return inspector.Graph;
        }
    }
}
