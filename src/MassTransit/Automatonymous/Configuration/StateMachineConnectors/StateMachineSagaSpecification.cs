namespace Automatonymous.StateMachineConnectors
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.SagaSpecifications;


    public class StateMachineSagaSpecification<TInstance> :
        SagaSpecification<TInstance>
        where TInstance : class, ISaga, SagaStateMachineInstance
    {
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineSagaSpecification(SagaStateMachine<TInstance> stateMachine, IEnumerable<ISagaMessageSpecification<TInstance>> messageSpecifications)
            : base(messageSpecifications)
        {
            _stateMachine = stateMachine;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            Observers.All(observer =>
            {
                observer.StateMachineSagaConfigured(this, _stateMachine);
                return true;
            });

            return base.Validate();
        }
    }
}
