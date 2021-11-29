namespace MassTransit.Context
{
    using Courier;


    public class HostCompensateActivityContext<TActivity, TLog> :
        CompensateContextProxy<TLog>,
        CompensateActivityContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly TActivity _activity;

        public HostCompensateActivityContext(TActivity activity, CompensateContext<TLog> context)
            : base(context)
        {
            _activity = activity;
        }

        TActivity CompensateActivityContext<TActivity, TLog>.Activity => _activity;
    }
}
