namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    public class MergePipe<TInput, TSplit> :
        IPipe<TSplit>
        where TSplit : class, PipeContext
        where TInput : class, PipeContext
    {
        readonly MergeFilterContextProvider<TInput, TSplit> _contextProvider;
        readonly TInput _input;
        readonly IPipe<TInput> _next;

        public MergePipe(IPipe<TInput> next, TInput input, MergeFilterContextProvider<TInput, TSplit> contextProvider)
        {
            _next = next;
            _input = input;
            _contextProvider = contextProvider;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new { InputType = TypeCache<TInput>.ShortName });

            _next.Probe(scope);
        }

        public Task Send(TSplit context)
        {
            var inputContext = _contextProvider(_input, context);

            return _next.Send(inputContext);
        }
    }
}
