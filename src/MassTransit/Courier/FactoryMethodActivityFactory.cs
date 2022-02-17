namespace MassTransit.Courier
{
    using System;
    using System.Threading.Tasks;


    public class FactoryMethodActivityFactory<TActivity, TArguments, TLog> :
        IActivityFactory<TActivity, TArguments, TLog>
        where TActivity : class, IExecuteActivity<TArguments>, ICompensateActivity<TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ICompensateActivityFactory<TActivity, TLog> _compensateFactory;
        readonly IExecuteActivityFactory<TActivity, TArguments> _executeFactory;

        public FactoryMethodActivityFactory(Func<TArguments, TActivity> executeFactory,
            Func<TLog, TActivity> compensateFactory)
        {
            _executeFactory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(executeFactory);
            _compensateFactory = new FactoryMethodCompensateActivityFactory<TActivity, TLog>(compensateFactory);
        }

        public Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            return _executeFactory.Execute(context, next);
        }

        public Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            return _compensateFactory.Compensate(context, next);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factoryMethod");
        }
    }
}
