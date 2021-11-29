namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Courier;


    public class CompensateActivityFactoryFilter<TActivity, TLog> :
        IFilter<CompensateContext<TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly ICompensateActivityFactory<TActivity, TLog> _factory;
        readonly IPipe<CompensateActivityContext<TActivity, TLog>> _pipe;

        public CompensateActivityFactoryFilter(ICompensateActivityFactory<TActivity, TLog> factory, IPipe<CompensateActivityContext<TActivity, TLog>> pipe)
        {
            _factory = factory;
            _pipe = pipe;
        }

        public async Task Send(CompensateContext<TLog> context, IPipe<CompensateContext<TLog>> next)
        {
            await _factory.Compensate(context, _pipe).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            _factory.Probe(context);
        }
    }
}
