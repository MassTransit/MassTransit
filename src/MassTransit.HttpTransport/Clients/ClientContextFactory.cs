namespace MassTransit.HttpTransport.Clients
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Pipeline;


    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IReceivePipe _receivePipe;

        public ClientContextFactory(IReceivePipe receivePipe)
        {
            _receivePipe = receivePipe;
        }

        public IPipeContextAgent<ClientContext> CreateContext(ISupervisor supervisor)
        {
            var client = new HttpClient();
            ClientContext clientContext = new HttpClientContext(client, _receivePipe, supervisor.Stopped);

            return supervisor.AddContext(clientContext);
        }

        public IActivePipeContextAgent<ClientContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<ClientContext> context,
            CancellationToken cancellationToken = default)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ClientContext> CreateSharedConnection(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedHttpClientContext(context.Result, cancellationToken)
                : new SharedHttpClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }
    }
}
