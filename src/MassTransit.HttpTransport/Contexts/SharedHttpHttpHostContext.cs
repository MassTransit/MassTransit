namespace MassTransit.HttpTransport.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Hosting;
    using Transport;


    public class SharedHttpHttpHostContext :
        ProxyPipeContext,
        HttpHostContext
    {
        readonly HttpHostContext _context;

        public SharedHttpHttpHostContext(HttpHostContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;

            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public HttpHostSettings HostSettings => _context.HostSettings;

        void HttpHostContext.RegisterEndpointHandler(string pathMatch, HttpConsumer handler)
        {
            _context.RegisterEndpointHandler(pathMatch, handler);
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return _context.Start(cancellationToken);
        }
    }
}
