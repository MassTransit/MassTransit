namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    public class SplitFilter<TInput, TSplit> :
        IFilter<TInput>
        where TSplit : class, PipeContext
        where TInput : class, PipeContext
    {
        readonly MergeFilterContextProvider<TInput, TSplit> _contextProvider;
        readonly FilterContextProvider<TSplit, TInput> _inputContextProvider;
        readonly IFilter<TSplit> _split;

        public SplitFilter(IFilter<TSplit> split, MergeFilterContextProvider<TInput, TSplit> contextProvider,
            FilterContextProvider<TSplit, TInput> inputContextProvider)
        {
            _split = split;
            _contextProvider = contextProvider;
            _inputContextProvider = inputContextProvider;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Set(new { SplitType = TypeCache<TSplit>.ShortName });

            _split.Probe(scope);
        }

        [DebuggerNonUserCode]
        public Task Send(TInput context, IPipe<TInput> next)
        {
            var mergePipe = new MergePipe<TInput, TSplit>(next, context, _contextProvider);

            return _split.Send(_inputContextProvider(context), mergePipe);
        }
    }
}
