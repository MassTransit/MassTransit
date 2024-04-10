namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Futures;
    using SagaStateMachine;


    public class FutureActivityConfigurator : IFutureActivityConfigurator
    {
        readonly FutureActivity _activity;

        public FutureActivityConfigurator(FutureActivity activity)
        {
            _activity = activity;
        }

        public void Then(Action<BehaviorContext<FutureState>> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _activity.Activity = new ActionActivity<FutureState>(action);
        }

        public void ThenAsync(Func<BehaviorContext<FutureState>, Task> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _activity.Activity = new AsyncActivity<FutureState>(action);
        }
    }
}
