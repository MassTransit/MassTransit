#nullable enable
namespace MassTransit.Transports
{
    using System;
    using Configuration;
    using Logging;
    using Middleware;
    using Observables;


    public abstract class BaseSendTransportContext :
        BasePipeContext,
        SendTransportContext
    {
        readonly Lazy<string> _activityName;
        readonly IHostConfiguration _hostConfiguration;

        protected BaseSendTransportContext(IHostConfiguration hostConfiguration, ISerialization serialization)
        {
            _hostConfiguration = hostConfiguration;

            SendObservers = new SendObservable();

            Serialization = serialization;

            _activityName = new Lazy<string>(() =>
            {
                var endpointName = EntityName;

                if (endpointName.Contains("_bus_"))
                    endpointName = "bus";

                return $"{endpointName} send";
            });
        }

        public abstract string EntityName { get; }

        public ILogContext LogContext => _hostConfiguration.SendLogContext ?? throw new InvalidOperationException("SendLogContext should not be null");

        public string ActivityName => _activityName.Value;

        public SendObservable SendObservers { get; }

        public ISerialization Serialization { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }
    }
}
