namespace MassTransit.Configuration
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using Pipeline;
    using Pipeline.Observables;


    public interface ISendEndpointConfiguration :
        IEndpointConfiguration
    {
        ISendPipe SendPipe { get; }

        Uri HostAddress { get; }

        Uri DestinationAddress { get; }

        SendObservable SendObservers { get; }

        ISendEndpoint Build();
    }
}
