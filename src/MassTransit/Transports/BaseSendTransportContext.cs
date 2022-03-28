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
        readonly Lazy<string> _destination;
        readonly IHostConfiguration _hostConfiguration;

        protected BaseSendTransportContext(IHostConfiguration hostConfiguration, ISerialization serialization)
        {
            _hostConfiguration = hostConfiguration;

            SendObservers = new SendObservable();

            Serialization = serialization;

            _destination = new Lazy<string>(() =>
            {
                var endpointName = EntityName;

                if (endpointName.Contains("_bus_"))
                    endpointName = "bus";
                else if (endpointName.Contains("_endpoint_"))
                    endpointName = "endpoint";
                else if (endpointName.Contains("_signalr_"))
                    endpointName = "signalr";
                else if (endpointName.StartsWith("Instance_"))
                    endpointName = "instance";

                return endpointName;
            });

            _activityName = new Lazy<string>(() => $"{_destination.Value} send");
        }

        public abstract string EntityName { get; }

        public ILogContext LogContext => _hostConfiguration.SendLogContext ?? throw new InvalidOperationException("SendLogContext should not be null");

        public string ActivityName => _activityName.Value;
        public string ActivityDestination => _destination.Value;
        public abstract string ActivitySystem { get; }

        public SendObservable SendObservers { get; }

        public ISerialization Serialization { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }
    }
}
