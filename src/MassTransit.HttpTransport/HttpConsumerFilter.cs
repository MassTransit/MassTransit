namespace MassTransit.HttpTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration.Builders;
    using Events;
    using GreenPipes;
    using Hosting;
    using Logging;
    using Pipeline;
    using Util;


    public class HttpConsumerFilter 
        : IFilter<OwinHostContext>
    {
        static readonly ILog _log = Logger.Get<HttpConsumerFilter>();

        readonly IReceiveTransportObserver _transportObserver;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ITaskSupervisor _supervisor;
        readonly HttpHostSettings _settings;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public HttpConsumerFilter(IPipe<ReceiveContext> receivePipe,
            IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver,
            ITaskSupervisor supervisor, 
            HttpHostSettings settings,
            ISendEndpointProvider sendEndpointProvider, 
            IPublishEndpointProvider publishEndpointProvider)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _settings = settings;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
        }

        public void Probe(ProbeContext context)
        {
            //no-op
        }

        public async Task Send(OwinHostContext context, IPipe<OwinHostContext> next)
        {
            //var receiveSettings = context.GetPayload<ReceiveSettings>();
            //var inputAddress = context.HostSettings.GetInputAddress(receiveSettings);
            var inputAddress = new Uri("http://localhost:8080");

            using (ITaskScope scope = _supervisor.CreateScope($"{TypeMetadataCache<HttpConsumerFilter>.ShortName} - {inputAddress}", () => TaskUtil.Completed))
            {
                var controller = new HttpConsumerAction(_receiveObserver, _settings, _receivePipe, scope, _sendEndpointProvider, _publishEndpointProvider);
                context.StartHttpListener(controller);

                await scope.Ready.ConfigureAwait(false);

                await _transportObserver.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

                scope.SetReady();

                try
                {
                    await scope.Completed.ConfigureAwait(false);
                }
                finally
                {
                    HttpConsumerMetrics metrics = controller;
                    await _transportObserver.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", metrics.ConsumerTag, metrics.DeliveryCount,
                            metrics.ConcurrentDeliveryCount);
                    }
                }
            }
        }
    }
}