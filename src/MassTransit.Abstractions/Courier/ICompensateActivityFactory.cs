namespace MassTransit
{
    using System.Threading.Tasks;


    public interface ICompensateActivityFactory<out TActivity, TLog> :
        IProbeSite
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next);
    }
}
