namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Mediator;


    public class MediatorRequestResponseTransport :
        IRequestResponseTransport
    {
        readonly IRequestResponseSettings _settings;
        IMediator _mediator;

        public MediatorRequestResponseTransport(IRequestResponseSettings settings)
        {
            _settings = settings;
        }

        public Task<IRequestClient<T>> GetRequestClient<T>(TimeSpan settingsRequestTimeout)
            where T : class
        {
            return Task.FromResult(_mediator.CreateRequestClient<T>(settingsRequestTimeout));
        }

        public void GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            _mediator = Bus.Factory.CreateMediator(callback);
        }

        public void Dispose()
        {
        }
    }
}