namespace MassTransit.HttpTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration.Builders;
    using Hosting;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Util;


    public class HttpConsumerFilter : IFilter<OwinHostContext>
    {
        static readonly ILog _log = Logger.Get<HttpConsumerFilter>();
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IReceiveObserver _receiveObserver;
        readonly IReceiveEndpointObserver _endpointObserver;
        readonly ITaskSupervisor _supervisor;


        public HttpConsumerFilter(IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver, IReceiveEndpointObserver endpointObserver,
            ITaskSupervisor supervisor)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _endpointObserver = endpointObserver;
            _supervisor = supervisor;
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
                var controller = new HttpConsumerAction(_receiveObserver, _receivePipe, scope);
                context.StartHttpListener(controller);

                await scope.Ready.ConfigureAwait(false);

                await _endpointObserver.Ready(new Ready(inputAddress)).ConfigureAwait(false);

                scope.SetReady();

                try
                {
                    await scope.Completed.ConfigureAwait(false);
                }
                finally
                {
                    HttpConsumerMetrics metrics = controller;
                    await _endpointObserver.Completed(new Completed(inputAddress, metrics)).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", metrics.ConsumerTag, metrics.DeliveryCount,
                            metrics.ConcurrentDeliveryCount);
                    }
                }
            }
        }

        class Ready :
          ReceiveEndpointReady
        {
            public Ready(Uri inputAddress)
            {
                InputAddress = inputAddress;
            }

            public Uri InputAddress { get; }
        }


        class Completed :
            ReceiveEndpointCompleted
        {
            public Completed(Uri inputAddress, HttpConsumerMetrics metrics)
            {
                InputAddress = inputAddress;
                DeliveryCount = metrics.DeliveryCount;
                ConcurrentDeliveryCount = metrics.ConcurrentDeliveryCount;
            }

            public Uri InputAddress { get; }
            public long DeliveryCount { get; }
            public long ConcurrentDeliveryCount { get; }
        }
    }
}