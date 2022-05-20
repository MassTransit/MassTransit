#nullable enable
namespace MassTransit.Transports
{
    using Logging;
    using Observables;


    public interface SendTransportContext :
        PipeContext,
        ISendObserverConnector
    {
        /// <summary>
        /// The LogContext used for sending transport messages, to ensure proper activity filtering
        /// </summary>
        ILogContext LogContext { get; }

        string ActivityName { get; }
        string ActivityDestination { get; }
        string ActivitySystem { get; }

        SendObservable SendObservers { get; }

        ISerialization Serialization { get; }
    }
}
