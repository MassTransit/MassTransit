namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public class GrpcMessageLatencyTransport :
        IMessageLatencyTransport
    {
        const string QueueName = "latency_consumer";

        readonly GrpcOptionSet _options;
        readonly IMessageLatencySettings _settings;
        IBusControl _busControl;
        IBusControl _outboundBus;
        ISendEndpoint _targetEndpoint;

        public GrpcMessageLatencyTransport(GrpcOptionSet options, IMessageLatencySettings settings)
        {
            _options = options;
            _settings = settings;
        }

        public Task Send(LatencyTestMessage message)
        {
            return _targetEndpoint.Send(message);
        }

        public async Task Start(Action<IReceiveEndpointConfigurator> callback, IReportConsumerMetric reportConsumerMetric)
        {
            _busControl = Bus.Factory.CreateUsingGrpc(x =>
            {
                x.Host(_options.HostAddress);

                x.ReceiveEndpoint(QueueName, e =>
                {
                    e.PrefetchCount = _settings.PrefetchCount;

                    if (_settings.ConcurrencyLimit > 0)
                        e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                    callback(e);
                });
            });

            await _busControl.StartAsync();

            var targetAddress = new Uri($"exchange:{QueueName}");

            if (_options.Split)
            {
                _outboundBus = Bus.Factory.CreateUsingGrpc(x =>
                {
                    x.Host(_options.SecondHostAddress, h =>
                    {
                        h.AddServer(_options.HostAddress);
                    });

                    if (_options.LoadBalance)
                    {
                        x.ReceiveEndpoint(QueueName, e =>
                        {
                            e.PrefetchCount = _settings.PrefetchCount;

                            if (_settings.ConcurrencyLimit > 0)
                                e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                            callback(e);
                        });
                    }
                });

                await Task.WhenAll(_busControl.StartAsync(), _outboundBus.StartAsync());

                _targetEndpoint = await _outboundBus.GetSendEndpoint(targetAddress);
            }
            else
            {
                await _busControl.StartAsync();
                _targetEndpoint = await _busControl.GetSendEndpoint(targetAddress);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();

            if (_outboundBus != null)
                await _outboundBus.StopAsync();
        }
    }
}
