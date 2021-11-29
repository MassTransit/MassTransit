namespace MassTransit
{
    using Courier;


    public interface CompensateActivityContext<out TLog> :
        CompensateContext<TLog>
        where TLog : class
    {
    }


    public interface CompensateActivityContext<out TActivity, out TLog> :
        CompensateActivityContext<TLog>
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        /// <summary>
        /// The activity that was created/used for this compensation
        /// </summary>
        TActivity Activity { get; }
    }
}
