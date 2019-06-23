namespace MassTransit.Context
{
    using GreenPipes;
    using Pipeline.Observables;


    public class BaseSendTransportContext :
        BasePipeContext,
        SendTransportContext
    {
        readonly SendObservable _sendObservers;

        protected BaseSendTransportContext(ILogContext logContext)
        {
            LogContext = logContext;

            _sendObservers = new SendObservable();
        }

        public ISendObserver SendObservers => _sendObservers;

        public ILogContext LogContext { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }
    }
}
