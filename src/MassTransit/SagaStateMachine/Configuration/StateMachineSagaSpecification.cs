namespace MassTransit.Configuration
{
    using System.Collections.Generic;


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
            Observers.ForEach(observer => observer.StateMachineSagaConfigured(this, _stateMachine));

            return base.Validate();
        }
    }
}
