namespace MassTransit.Futures
{
    using System.Collections.Generic;
    using SagaStateMachine;


    public class FutureActivity : ISpecification
    {
        public FutureActivity()
        {
            Action = DefaultActionActivity;
        }

        public ActionActivity<FutureState> Action { set; get; }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        static readonly ActionActivity<FutureState> DefaultActionActivity = new (_ =>
        {
        });
    }
}
