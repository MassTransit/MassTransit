namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;
    using Pipeline.Observables;
    using Transports;


    public interface IReceiveEndpointConfiguration :
        IEndpointConfiguration,
        IReceiveEndpointObserverConnector
    {
        IConsumePipe ConsumePipe { get; }

        Uri HostAddress { get; }

        Uri InputAddress { get; }

        bool ConfigureConsumeTopology { get; }

        ReceiveEndpointObservable EndpointObservers { get; }
        ReceiveObservable ReceiveObservers { get; }
        ReceiveTransportObservable TransportObservers { get; }
        IReceiveEndpoint ReceiveEndpoint { get; }

        /// <summary>
        /// Completed once the receive endpoint dependencies are ready
        /// </summary>
        Task Dependencies { get; }

        /// <summary>
        /// Create the receive pipe, using the endpoint configuration
        /// </summary>
        /// <returns></returns>
        IReceivePipe CreateReceivePipe();
    }
}
