namespace MassTransit.Testing
{
    using Automatonymous;
    using Saga;


    public class StateMachineSagaTestHarness<TInstance, TStateMachine> :
        SagaTestHarness<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TStateMachine :  SagaStateMachine<TInstance>
    {
        readonly TStateMachine _stateMachine;

        public StateMachineSagaTestHarness(BusTestHarness testHarness, ISagaRepository<TInstance> repository, TStateMachine stateMachine, string queueName)
            : base(testHarness, repository, queueName)
        {
            _stateMachine = stateMachine;
        }

        protected override void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_stateMachine, TestRepository);
        }

        protected override void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                x.StateMachineSaga(_stateMachine, TestRepository);
            });
        }
    }
}
