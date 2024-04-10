namespace MassTransit.Futures
{
    using System.Collections.Generic;
    using SagaStateMachine;


    public class FutureActivity : ISpecification
    {
        public FutureActivity()
        {
            _activity = DefaultActionActivity;
        }

        IStateMachineActivity<FutureState> _activity;

        public IStateMachineActivity<FutureState> Activity
        {
            set => _activity = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public EventActivityBinder<FutureState, T> Execute<T>(EventActivityBinder<FutureState, T> eventActivityBinder)
            where T : class
        {
            return eventActivityBinder.Execute(_ => _activity);
        }

        static readonly ActionActivity<FutureState> DefaultActionActivity = new(_ =>
        {
        });
    }
}
