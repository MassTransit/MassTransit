namespace MassTransit.Context
{
    using GreenPipes;
    using Pipeline.Observables;


    public interface SendTransportContext :
        PipeContext,
        ISendObserverConnector
    {
        SendObservable SendObservers { get; }

        /// <summary>
        /// The LogContext used for sending transport messages, to ensure proper activity filtering
        /// </summary>
        ILogContext LogContext { get; }
    }
}
