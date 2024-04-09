namespace MassTransit.Configuration
{
    using System;
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

            _activity.Action = new ActionActivity<FutureState>(action);
        }
    }
}
