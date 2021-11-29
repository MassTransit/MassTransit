namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    public class EmptyPipe<TContext> :
        IPipe<TContext>
        where TContext : class, PipeContext
    {
        [DebuggerNonUserCode]
        Task IPipe<TContext>.Send(TContext context)
        {
            return Task.CompletedTask;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }
    }
}
