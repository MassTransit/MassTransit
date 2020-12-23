namespace MassTransit.Context
{
    using Configuration;
    using GreenPipes;
    using Pipeline.Observables;


    public class BaseSendTransportContext :
        BasePipeContext,
        SendTransportContext
    {
        readonly IHostConfiguration _hostConfiguration;

        protected BaseSendTransportContext(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            SendObservers = new SendObservable();
        }

        public ILogContext LogContext => _hostConfiguration.SendLogContext;

        public SendObservable SendObservers { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }
    }
}
