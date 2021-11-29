namespace MassTransit.Context
{
    using Courier;


    public class HostExecuteActivityContext<TActivity, TArguments> :
        ExecuteContextProxy<TArguments>,
        ExecuteActivityContext<TActivity, TArguments>
        where TArguments : class
        where TActivity : class, IExecuteActivity<TArguments>
    {
        readonly TActivity _activity;

        public HostExecuteActivityContext(TActivity activity, ExecuteContext<TArguments> context)
            : base(context)
        {
            _activity = activity;
        }

        TActivity ExecuteActivityContext<TActivity, TArguments>.Activity => _activity;
    }
}
