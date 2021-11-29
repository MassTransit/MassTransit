namespace MassTransit.Clients
{
    using System;
    using System.Threading.Tasks;


    public class HostReceiveEndpointClientFactoryContext :
        ReceiveEndpointClientFactoryContext,
        IAsyncDisposable
    {
        readonly HostReceiveEndpointHandle _receiveEndpointHandle;

        public HostReceiveEndpointClientFactoryContext(HostReceiveEndpointHandle receiveEndpointHandle, ReceiveEndpointReady receiveEndpointReady,
            RequestTimeout defaultTimeout = default)
            : base(receiveEndpointReady, defaultTimeout)
        {
            _receiveEndpointHandle = receiveEndpointHandle;
        }

        public async ValueTask DisposeAsync()
        {
            await _receiveEndpointHandle.StopAsync().ConfigureAwait(false);
        }
    }
}
