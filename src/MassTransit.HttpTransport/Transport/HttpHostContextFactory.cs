namespace MassTransit.HttpTransport.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;


    public class HttpHostContextFactory :
        IPipeContextFactory<HttpHostContext>
    {
        readonly IHttpHostConfiguration _configuration;

        public HttpHostContextFactory(IHttpHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        IPipeContextAgent<HttpHostContext> IPipeContextFactory<HttpHostContext>.CreateContext(ISupervisor supervisor)
        {
            HttpHostContext hostContext = new KestrelHttpHostContext(_configuration, supervisor.Stopped);

            hostContext.GetOrAddPayload(() => _configuration);

            IPipeContextAgent<HttpHostContext> contextHandle = supervisor.AddContext(hostContext);

            return contextHandle;
        }

        IActivePipeContextAgent<HttpHostContext> IPipeContextFactory<HttpHostContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<HttpHostContext> context,
            CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<HttpHostContext> CreateSharedConnection(Task<HttpHostContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedHttpHttpHostContext(context.Result, cancellationToken)
                : new SharedHttpHttpHostContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }
    }
}
