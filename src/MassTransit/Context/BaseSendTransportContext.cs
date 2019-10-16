namespace MassTransit.Context
{
    using GreenPipes;
    using Pipeline.Observables;


    public class BaseSendTransportContext :
        BasePipeContext,
        SendTransportContext
    {
        protected BaseSendTransportContext(ILogContext logContext)
        {
            LogContext = logContext;

            SendObservers = new SendObservable();
        }

        public ILogContext LogContext { get; }
        public SendObservable SendObservers { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }
    }
}
