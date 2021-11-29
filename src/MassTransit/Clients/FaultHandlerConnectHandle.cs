namespace MassTransit.Clients
{
    using System;
    using System.Threading;


    /// <summary>
    /// The fault handler for the request client
    /// </summary>
    public class FaultHandlerConnectHandle :
        HandlerConnectHandle
    {
        readonly ConnectHandle _handle;

        public FaultHandlerConnectHandle(ConnectHandle handle)
        {
            _handle = handle;
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public void Disconnect()
        {
            _handle.Disconnect();
        }

        public void TrySetException(Exception exception)
        {
        }

        public void TrySetCanceled(CancellationToken cancellationToken)
        {
        }
    }
}
