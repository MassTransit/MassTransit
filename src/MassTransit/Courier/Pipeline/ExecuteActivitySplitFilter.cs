namespace MassTransit.Courier.Pipeline
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Util;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public class ExecuteActivitySplitFilter<TActivity, TArguments> :
        IFilter<ExecuteActivityContext<TActivity, TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IFilter<ExecuteActivityContext<TArguments>> _next;

        public ExecuteActivitySplitFilter(IFilter<ExecuteActivityContext<TArguments>> next)
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
        public Task Send(ExecuteActivityContext<TActivity, TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            var mergePipe = new ExecuteActivityMergePipe<TActivity, TArguments>(next);

            return _next.Send(context, mergePipe);
        }
    }
}
