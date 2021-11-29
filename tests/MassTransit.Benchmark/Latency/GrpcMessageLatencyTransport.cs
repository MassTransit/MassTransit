namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public class GrpcMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly GrpcOptionSet _options;
        readonly IMessageLatencySettings _settings;
        IBusControl _busControl;
        IBusControl _outboundBus;
        string _queueName = "latency_consumer";
        Task<ISendEndpoint> _targetEndpoint;

        public GrpcMessageLatencyTransport(GrpcOptionSet options, IMessageLatencySettings settings)
        {
            _options = options;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingGrpc(x =>
            {
                x.Host(_options.HostAddress);

                x.ReceiveEndpoint(_queueName, e =>
                {
                    e.PrefetchCount = _settings.PrefetchCount;

                    if (_settings.ConcurrencyLimit > 0)
                        e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                    callback(e);
                });
            });

            await _busControl.StartAsync();

            var targetAddress = new Uri($"exchange:{_queueName}");

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
                        x.ReceiveEndpoint(_queueName, e =>
                        {
                            e.PrefetchCount = _settings.PrefetchCount;

                            if (_settings.ConcurrencyLimit > 0)
                                e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                            callback(e);
                        });
                    }
                });

                await Task.WhenAll(_busControl.StartAsync(), _outboundBus.StartAsync());

                _targetEndpoint = _outboundBus.GetSendEndpoint(targetAddress);
            }
            else
            {
                await _busControl.StartAsync();
                _targetEndpoint = _busControl.GetSendEndpoint(targetAddress);
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