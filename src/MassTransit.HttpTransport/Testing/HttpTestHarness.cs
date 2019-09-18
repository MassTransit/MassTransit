namespace MassTransit.HttpTransport.Testing
{
    using System;
    using System.Net.Http;
    using MassTransit.Testing;


    public class HttpTestHarness :
        BusTestHarness
    {
        Uri _inputQueueAddress;

        public HttpTestHarness(Uri hostAddress = null, Uri inputQueueAddress = null)
        {
            HostAddress = hostAddress ?? new Uri("http://localhost:8080");

            _inputQueueAddress = inputQueueAddress ?? HostAddress;

            InputQueueName = _inputQueueAddress.AbsolutePath.Trim('/');
        }

        public Uri HostAddress { get; }

        public override string InputQueueName { get; }
        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IHttpBusFactoryConfigurator> OnConfigureHttpBus;
        public event Action<IHttpReceiveEndpointConfigurator> OnConfigureHttpReceiveEndpoint;

        protected virtual void ConfigureHttpBus(IHttpBusFactoryConfigurator configurator)
        {
            OnConfigureHttpBus?.Invoke(configurator);
        }


        protected virtual void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            OnConfigureHttpReceiveEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingHttp(x =>
            {
                ConfigureBus(x);

                ConfigureHttpBus(x);

                x.Host(HostAddress, h => h.Method = HttpMethod.Post);

                x.ReceiveEndpoint("", e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureHttpReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}
