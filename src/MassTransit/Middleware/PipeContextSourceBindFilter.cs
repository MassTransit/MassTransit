namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Binds a context to the pipe using a <see cref="IPipeContextSource{TSource}" />.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public class PipeContextSourceBindFilter<TLeft, TRight> :
        IFilter<TLeft>
        where TLeft : class, PipeContext
        where TRight : class, PipeContext
    {
        readonly IPipe<BindContext<TLeft, TRight>> _output;
        readonly IPipeContextSource<TRight, TLeft> _source;

        public PipeContextSourceBindFilter(IPipe<BindContext<TLeft, TRight>> output, IPipeContextSource<TRight, TLeft> source)
        {
            _output = output;
            _source = source;
        }

        public Task Send(TLeft context, IPipe<TLeft> next)
        {
            var bindPipe = new BindPipe(context, _output);

            var sourceTask = _source.Send(context, bindPipe);
            if (sourceTask.Status == TaskStatus.RanToCompletion)
                return next.Send(context);

            async Task SendAsync()
            {
                await sourceTask.ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }

            return SendAsync();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("bind");
            _output.Probe(scope);
            _source.Probe(scope);
        }


        class BindPipe :
            IPipe<TRight>
        {
            readonly TLeft _context;
            readonly IPipe<BindContext<TLeft, TRight>> _output;

            public BindPipe(TLeft context, IPipe<BindContext<TLeft, TRight>> output)
            {
                _context = context;
                _output = output;
            }

            public Task Send(TRight context)
            {
                var bindContext = new BindContextProxy<TLeft, TRight>(_context, context);

                return _output.Send(bindContext);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
