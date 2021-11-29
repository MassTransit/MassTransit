#nullable enable
namespace MassTransit.Transports
{
    using Configuration;
    using Logging;
    using Middleware;
    using Observables;


    public class BaseSendTransportContext :
        BasePipeContext,
        SendTransportContext
    {
        readonly IHostConfiguration _hostConfiguration;

        protected BaseSendTransportContext(IHostConfiguration hostConfiguration, ISerialization serialization)
        {
            _hostConfiguration = hostConfiguration;

            SendObservers = new SendObservable();

            Serialization = serialization;
        }

        public ILogContext LogContext => _hostConfiguration.SendLogContext;

        public SendObservable SendObservers { get; }

        public ISerialization Serialization { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }
    }
}
