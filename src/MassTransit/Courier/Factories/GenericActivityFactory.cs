namespace MassTransit.Courier.Factories
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class GenericActivityFactory<TActivity, TArguments, TLog> :
        IActivityFactory<TActivity, TArguments, TLog>
        where TActivity : class, IExecuteActivity<TArguments>, ICompensateActivity<TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IActivityFactory _activityFactory;

        public GenericActivityFactory(IActivityFactory activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public Task<ResultContext<ExecutionResult>> Execute(ExecuteContext<TArguments> context,
            IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> next)
        {
            return _activityFactory.Execute(context, next);
        }

        public Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
        {
            return _activityFactory.Compensate(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _activityFactory.Probe(context);
        }
    }
}
