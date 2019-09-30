namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface ICompensateActivityFactory<out TActivity, TLog> :
        IProbeSite
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next);
    }
}
