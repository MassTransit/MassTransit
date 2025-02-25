namespace MassTransit.Clients
{
    using System;
    using System.Threading.Tasks;


    public class HostReceiveEndpointClientFactoryContext :
        ReceiveEndpointClientFactoryContext,
        IAsyncDisposable
    {
        readonly HostReceiveEndpointHandle _handle;

        public HostReceiveEndpointClientFactoryContext(HostReceiveEndpointHandle handle, RequestTimeout defaultTimeout = default)
            : base(handle, defaultTimeout)
        {
            _handle = handle;
        }

        public async ValueTask DisposeAsync()
        {
            await _handle.StopAsync().ConfigureAwait(false);
        }
    }
}
