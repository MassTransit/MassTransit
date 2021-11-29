namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Forks a single pipe into two pipes, which are executed concurrently
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ForkFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IPipe<TContext> _pipe;

        public ForkFilter(IPipe<TContext> pipe)
        {
            _pipe = pipe;
        }

        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            return Task.WhenAll(_pipe.Send(context), next.Send(context));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("fork");
            _pipe.Probe(scope);
        }
    }
}
