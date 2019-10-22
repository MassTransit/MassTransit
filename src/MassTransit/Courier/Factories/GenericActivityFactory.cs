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

        public Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            return _activityFactory.Execute(context, next);
        }

        public Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            return _activityFactory.Compensate(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _activityFactory.Probe(context);
        }
    }
}
