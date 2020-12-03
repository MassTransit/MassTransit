namespace MassTransit
{
    using System.Threading;
    using Transports;


    public interface IReceiveEndpointControl :
        IReceiveEndpoint
    {
        ReceiveEndpointHandle Start(CancellationToken cancellationToken);
    }
}
