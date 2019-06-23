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
        readonly IList<ConnectHandle> _handles;
        readonly Lazy<Uri> _instanceServiceEndpointAddress;
        readonly IDictionary<Type, IMessageEndpoint> _messageTypes;
        readonly Lazy<Uri> _serviceEndpointAddress;

        public ServiceEndpoint(IServiceInstance instance, IReceiveEndpointConfigurator endpointConfigurator,
            IReceiveEndpointConfigurator instanceEndpointConfigurator)
        {
            _instance = instance;

            endpointConfigurator.ConnectReceiveEndpointObserver(new UpDownObserver(this));

            _handles = new List<ConnectHandle>();
            _messageTypes = new Dictionary<Type, IMessageEndpoint>();

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

        public void ConnectObservers(IConsumePipeConfigurator configurator)
        {
            _handles.Add(configurator.ConnectConsumerConfigurationObserver(new ServiceEndpointConfigurationObserver(configurator, this)));
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

        void DisconnectObservers()
        {
            foreach (var handle in _handles)
            {
                handle.Disconnect();
            }

            _handles.Clear();
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
                _serviceEndpoint.DisconnectObservers();

                Task.Run(() => _serviceEndpoint.NotifyUp());

                return TaskUtil.Completed;
            }

            public Task Completed(ReceiveEndpointCompleted completed)
            {
                Task.Run(() => _serviceEndpoint.NotifyDown());

                return TaskUtil.Completed;
            }

            public Task Faulted(ReceiveEndpointFaulted faulted)
            {
                _serviceEndpoint.DisconnectObservers();
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
