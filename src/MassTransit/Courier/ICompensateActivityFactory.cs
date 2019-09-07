namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface ICompensateActivityFactory<out TActivity, TLog> :
        IProbeSite
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next);
    }
}
