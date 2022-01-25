namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Mediator;


    public class MediatorMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly IMessageLatencySettings _settings;
        IMediator _mediator;
        Task<ISendEndpoint> _targetEndpoint;

        public MediatorMessageLatencyTransport(IMessageLatencySettings settings)
        {
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _mediator = Bus.Factory.CreateMediator(callback);

            _targetEndpoint = Task.FromResult<ISendEndpoint>(_mediator);

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_mediator is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }
    }
}
