namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Threading;


    public interface IRiderControl :
        IRider
    {
        RiderHandle Start(CancellationToken cancellationToken = default);

        IEnumerable<EndpointHealthResult> CheckEndpointHealth();
    }
}
