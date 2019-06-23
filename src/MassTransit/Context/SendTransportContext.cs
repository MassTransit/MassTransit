namespace MassTransit.Context
{
    using GreenPipes;


    public interface SendTransportContext :
        PipeContext,
        ISendObserverConnector
    {
        ISendObserver SendObservers { get; }

        /// <summary>
        /// The LogContext used for sending transport messages, to ensure proper activity filtering
        /// </summary>
        ILogContext LogContext { get; }
    }
}
