namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Intercepts the pipe and executes an adjacent pipe prior to executing the next filter in the main pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class InterceptFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IPipe<TContext> _pipe;

        public InterceptFilter(IPipe<TContext> pipe)
        {
            _pipe = pipe;
        }

        async Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            await _pipe.Send(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("wiretap");

            _pipe.Probe(scope);
        }
    }
}
