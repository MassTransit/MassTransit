namespace MassTransit.Courier.Pipeline
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class CompensateActivitySplitFilter<TActivity, TLog> :
        IFilter<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly IFilter<CompensateActivityContext<TLog>> _next;

        public CompensateActivitySplitFilter(IFilter<CompensateActivityContext<TLog>> next)
        {
            _next = next;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Add("activityType", TypeMetadataCache<TActivity>.ShortName);

            _next.Probe(scope);
        }

        [DebuggerNonUserCode]
        public Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            var mergePipe = new CompensateActivityMergePipe<TActivity, TLog>(next);

            return _next.Send(context, mergePipe);
        }
    }
}
