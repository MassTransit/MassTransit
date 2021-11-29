namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Courier;


    public class ExecuteActivityFactoryFilter<TActivity, TArguments> :
        IFilter<ExecuteContext<TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityFactory<TActivity, TArguments> _factory;
        readonly IPipe<ExecuteActivityContext<TActivity, TArguments>> _pipe;

        public ExecuteActivityFactoryFilter(IExecuteActivityFactory<TActivity, TArguments> factory, IPipe<ExecuteActivityContext<TActivity, TArguments>> pipe)
        {
            _factory = factory;
            _pipe = pipe;
        }

        public async Task Send(ExecuteContext<TArguments> context, IPipe<ExecuteContext<TArguments>> next)
        {
            await _factory.Execute(context, _pipe).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            _factory.Probe(context);
        }
    }
}
