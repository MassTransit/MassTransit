namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Consumers;
    using Context;
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Specifications;
    using Pipeline;
    using Util;


    public class ServiceEndpoint :
        IServiceEndpoint
    {
        readonly IServiceInstance _instance;
        readonly TaskCompletionSource<InstanceInfo> _instanceInfo;
        readonly TaskCompletionSource<IPublishEndpointProvider> _publishEndpointProvider;
        readonly TaskCompletionSource<ISendEndpointProvider> _sendEndpointProvider;
        readonly Lazy<Uri> _serviceAddress;
        readonly TaskCompletionSource<ServiceInfo> _serviceInfo;

        public ServiceEndpoint(IServiceInstance instance, IReceiveEndpointConfigurator endpointConfigurator, IServiceEndpointClientCache clientCache)
        {
            _instance = instance;

            endpointConfigurator.ConnectReceiveEndpointObserver(new ServiceEndpointObserver(this));

            _sendEndpointProvider = TaskUtil.GetTask<ISendEndpointProvider>();
            _publishEndpointProvider = TaskUtil.GetTask<IPublishEndpointProvider>();

            _serviceAddress = new Lazy<Uri>(() => endpointConfigurator.InputAddress);
            _serviceInfo = TaskUtil.GetTask<ServiceInfo>();
            _instanceInfo = TaskUtil.GetTask<InstanceInfo>();

            ClientCache = clientCache;
        }

        DateTime? Started { get; set; }

        IServiceEndpointClientCache ClientCache { get; }
        public Task<InstanceInfo> InstanceInfo => _instanceInfo.Task;
        public Task<ServiceInfo> ServiceInfo => _serviceInfo.Task;

        public void ConfigureServiceEndpoint<T>(IConsumePipeConfigurator configurator)
            where T : class
        {
            var messageCache = ClientCache.GetMessageCache<T>();

            IFilter<ConsumeContext<T>> filter = new ServiceEndpointMessageFilter<T>(messageCache);

            configurator.AddPipeSpecification(new FilterPipeSpecification<ConsumeContext<T>>(filter));
        }

        public void ConfigureControlEndpoint<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            var messageCache = ClientCache.GetMessageCache<T>();

            configurator.Consumer(new LinkConsumerFactory<T>(this, messageCache));
            configurator.Consumer(new UnlinkConsumerFactory<T>(messageCache));
        }

        public async Task NotifyEndpointReady(IReceiveEndpoint receiveEndpoint)
        {
            Started = DateTime.UtcNow;

            _sendEndpointProvider.TrySetResult(receiveEndpoint);
            _publishEndpointProvider.TrySetResult(receiveEndpoint);

            var serviceInfo = new ServiceEndpointServiceInfo(_serviceAddress.Value);
            _serviceInfo.TrySetResult(serviceInfo);

            var instanceInfo = new ServiceEndpointInstanceInfo(_instance.InstanceId, Started);
            _instanceInfo.TrySetResult(instanceInfo);

            try
            {
                await ClientCache.NotifyEndpointReady(receiveEndpoint, instanceInfo, serviceInfo).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Failed to notify service endpoint down: {ServiceAddress} {InstanceId}", serviceInfo.ServiceAddress,
                    instanceInfo.InstanceId);
            }
        }

        public async Task NotifyDown()
        {
            try
            {
                var instanceInfo = _instanceInfo.Task.IsCompletedSuccessfully() ? _instanceInfo.Task.Result : await _instanceInfo.Task.ConfigureAwait(false);
                var serviceInfo = _serviceInfo.Task.IsCompletedSuccessfully() ? _serviceInfo.Task.Result : await _serviceInfo.Task.ConfigureAwait(false);

                var message = new InstanceDownMessage(instanceInfo, serviceInfo);

                await NotifyClients<InstanceDown>(message).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Failed to notify service endpoint down: {ServiceAddress}", _serviceAddress.Value);
            }
        }

        Task NotifyClients<T>(T message)
        {
            return ClientCache.ForEachAsync(client => NotifyClient(message, client));
        }

        async Task NotifyClient<T>(T message, Task<ServiceClientContext> asyncContext)
        {
            ServiceClientContext clientContext = null;
            try
            {
                clientContext = await asyncContext.ConfigureAwait(false);

                var sendEndpointProvider = await _sendEndpointProvider.Task.ConfigureAwait(false);

                var endpoint = await sendEndpointProvider.GetSendEndpoint(clientContext.Address).ConfigureAwait(false);

                await endpoint.Send(message).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (clientContext != null)
                    ClientCache.NotifyFaulted(clientContext, exception);
            }
        }


        class InstanceDownMessage :
            InstanceDown
        {
            public InstanceDownMessage(InstanceInfo instance, ServiceInfo service)
            {
                Instance = instance;
                Service = service;
            }

            public InstanceInfo Instance { get; }
            public ServiceInfo Service { get; }
        }


        class ServiceEndpointServiceInfo :
            ServiceInfo
        {
            public ServiceEndpointServiceInfo(Uri serviceAddress)
            {
                ServiceAddress = serviceAddress;
            }

            public Uri ServiceAddress { get; }

            public ServiceCapability[] Capabilities => default;
        }


        class ServiceEndpointInstanceInfo :
            InstanceInfo
        {
            public ServiceEndpointInstanceInfo(Guid instanceId, DateTime? started)
            {
                InstanceId = instanceId;
                Started = started;
            }

            public Guid InstanceId { get; }

            public DateTime? Started { get; }
        }
    }
}
