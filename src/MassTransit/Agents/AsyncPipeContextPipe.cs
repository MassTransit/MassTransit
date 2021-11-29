namespace MassTransit.Agents
{
    using System.Threading.Tasks;


    /// <summary>
    /// Completes the AsyncPipeContextAgent when the context is sent to the pipe, and doesn't return until the agent completes
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncPipeContextPipe<TContext> :
        IPipe<TContext>
        where TContext : class, PipeContext
    {
        readonly IAsyncPipeContextAgent<TContext> _agent;
        readonly IPipe<TContext> _pipe;

        public AsyncPipeContextPipe(IAsyncPipeContextAgent<TContext> agent, IPipe<TContext> pipe)
        {
            _agent = agent;
            _pipe = pipe;
        }

        public async Task Send(TContext context)
        {
            await _pipe.Send(context).ConfigureAwait(false);

            await _agent.Created(context).ConfigureAwait(false);

            await _agent.Completed.ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }
    }
}
