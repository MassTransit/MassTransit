namespace MassTransit.Conductor.Server
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Consumers;
    using Contracts;
    using Util;


    public class ServiceInstance :
        IServiceInstance
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly Lazy<Uri> _instanceAddress;
        readonly IDictionary<Type, IMessageEndpoint> _messageTypes;
        readonly TaskCompletionSource<IReceiveEndpoint> _receiveEndpoint;

        public ServiceInstance(NewId instanceId, IReceiveEndpointConfigurator configurator)
        {
            InstanceId = instanceId;
            _configurator = configurator;

            EndpointId = instanceId.ToGuid();
            _instanceAddress = new Lazy<Uri>(() => configurator.InputAddress);

            _messageTypes = new Dictionary<Type, IMessageEndpoint>();
            _receiveEndpoint = new TaskCompletionSource<IReceiveEndpoint>();

            configurator.ConnectReceiveEndpointObserver(new InstanceReadyObserver(this));
        }

        public NewId InstanceId { get; }
        public Guid EndpointId { get; }
        public Uri InstanceAddress => _instanceAddress.Value;

        public void ConfigureMessageEndpoint<T>(IMessageEndpoint<T> endpoint)
            where T : class
        {
            if (_messageTypes.ContainsKey(typeof(T)))
                throw new ArgumentException($"The message type was already added by another service endpoint: {TypeMetadataCache<T>.ShortName}", nameof(T));

            _messageTypes.Add(typeof(T), endpoint);

            _configurator.Consumer(() => new LinkConsumer<T>(endpoint));
        }

        public async Task NotifyUp<T>(IMessageEndpoint<T> endpoint)
            where T : class
        {
            var receiveEndpoint = await _receiveEndpoint.Task.ConfigureAwait(false);

            await receiveEndpoint.CreatePublishEndpoint(_instanceAddress.Value).Publish<Up<T>>(new
            {
                endpoint.ServiceAddress,
                Endpoint = endpoint.EndpointInfo
            }).ConfigureAwait(false);
        }

        public async Task NotifyDown<T>(IMessageEndpoint<T> endpoint)
            where T : class
        {
            var receiveEndpoint = await _receiveEndpoint.Task.ConfigureAwait(false);

            await receiveEndpoint.CreatePublishEndpoint(_instanceAddress.Value).Publish<Down<T>>(new
            {
                endpoint.ServiceAddress,
                Endpoint = endpoint.EndpointInfo
            }).ConfigureAwait(false);
        }


        class InstanceReadyObserver :
            IReceiveEndpointObserver
        {
            readonly ServiceInstance _instance;

            public InstanceReadyObserver(ServiceInstance instance)
            {
                _instance = instance;
            }

            public Task Ready(ReceiveEndpointReady ready)
            {
                _instance._receiveEndpoint.TrySetResult(ready.ReceiveEndpoint);

                return TaskUtil.Completed;
            }

            public Task Completed(ReceiveEndpointCompleted completed)
            {
                return TaskUtil.Completed;
            }

            public Task Faulted(ReceiveEndpointFaulted faulted)
            {
                _instance._receiveEndpoint.TrySetException(faulted.Exception);

                return TaskUtil.Completed;
            }
        }
    }
}
