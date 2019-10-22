namespace MassTransit.Conductor.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Util;
    using Observers;


    public class ServiceEndpoint :
        IServiceEndpoint
    {
        readonly IServiceInstance _instance;
        readonly Lazy<EndpointInfo> _endpointInfo;
        readonly IList<ConnectHandle> _configurationHandles;
        readonly Lazy<Uri> _instanceServiceEndpointAddress;
        readonly IDictionary<Type, IMessageEndpoint> _messageTypes;
        readonly Lazy<Uri> _serviceEndpointAddress;

        public ServiceEndpoint(IServiceInstance instance, IReceiveEndpointConfigurator endpointConfigurator,
            IReceiveEndpointConfigurator instanceEndpointConfigurator)
        {
            _instance = instance;

            _configurationHandles = new List<ConnectHandle>();
            _messageTypes = new Dictionary<Type, IMessageEndpoint>();

            endpointConfigurator.ConnectReceiveEndpointObserver(new UpDownObserver(this));

            _serviceEndpointAddress = new Lazy<Uri>(() => endpointConfigurator.InputAddress);
            _instanceServiceEndpointAddress = new Lazy<Uri>(() => instanceEndpointConfigurator.InputAddress);
            _endpointInfo = new Lazy<EndpointInfo>(CreateEndpointInfo);
        }

        public Uri ServiceAddress => _serviceEndpointAddress.Value;
        public EndpointInfo EndpointInfo => _endpointInfo.Value;

        public IMessageEndpoint<T> GetMessageEndpoint<T>()
            where T : class
        {
            if (!_messageTypes.TryGetValue(typeof(T), out var endpoint))
            {
                var messageEndpoint = new MessageEndpoint<T>(this);

                _messageTypes.Add(typeof(T), messageEndpoint);

                _instance.ConfigureMessageEndpoint(messageEndpoint);

                return messageEndpoint;
            }

            return endpoint as IMessageEndpoint<T>;
        }

        public void ConnectConfigurationObserver(IConsumePipeConfigurator configurator)
        {
            var configurationObserver = new ServiceEndpointConfigurationObserver(configurator, this);

            _configurationHandles.Add(configurator.ConnectConsumerConfigurationObserver(configurationObserver));
            _configurationHandles.Add(configurator.ConnectHandlerConfigurationObserver(configurationObserver));
            _configurationHandles.Add(configurator.ConnectSagaConfigurationObserver(configurationObserver));
            _configurationHandles.Add(configurator.ConnectActivityConfigurationObserver(configurationObserver));
        }

        EndpointInfo CreateEndpointInfo()
        {
            return new Endpoint
            {
                EndpointId = _instance.EndpointId,
                Started = Started ?? DateTime.UtcNow,
                InstanceAddress = _instance.InstanceAddress,
                EndpointAddress = _instanceServiceEndpointAddress.Value
            };
        }

        DateTime? Started { get; set; }

        void DisconnectConfigurationObservers()
        {
            foreach (var handle in _configurationHandles)
            {
                handle.Disconnect();
            }

            _configurationHandles.Clear();
        }

        async Task NotifyUp()
        {
            try
            {
                await Task.WhenAll(_messageTypes.Values.Select(x => x.NotifyUp(_instance))).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Failed to notify service endpoint up: {InstanceAddress}", EndpointInfo.InstanceAddress);
            }
        }

        async Task NotifyDown()
        {
            try
            {
                await Task.WhenAll(_messageTypes.Values.Select(x => x.NotifyDown(_instance))).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Failed to notify service endpoint down: {InstanceAddress}", EndpointInfo.InstanceAddress);
            }
        }


        class UpDownObserver :
            IReceiveEndpointObserver
        {
            readonly ServiceEndpoint _serviceEndpoint;

            public UpDownObserver(ServiceEndpoint serviceEndpoint)
            {
                _serviceEndpoint = serviceEndpoint;
            }

            public Task Ready(ReceiveEndpointReady ready)
            {
                _serviceEndpoint.Started = DateTime.UtcNow;
                _serviceEndpoint.DisconnectConfigurationObservers();

                return _serviceEndpoint.NotifyUp();
            }

            public Task Stopping(ReceiveEndpointStopping stopping)
            {
                return _serviceEndpoint.NotifyDown();
            }

            public Task Completed(ReceiveEndpointCompleted completed)
            {
                return TaskUtil.Completed;
            }

            public Task Faulted(ReceiveEndpointFaulted faulted)
            {
                _serviceEndpoint.DisconnectConfigurationObservers();

                return TaskUtil.Completed;
            }
        }


        class Endpoint :
            EndpointInfo
        {
            public Guid EndpointId { get; set; }
            public DateTime Started { get; set; }
            public Uri InstanceAddress { get; set; }
            public Uri EndpointAddress { get; set; }
        }
    }
}
