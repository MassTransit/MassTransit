namespace MassTransit.HttpTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class HttpTestFixture :
        BusTestFixture
    {
        public HttpTestFixture()
            : this(new HttpTestHarness(new Uri("http://localhost:8080")))
        {
        }

        public HttpTestFixture(HttpTestHarness harness)
            : base(harness)
        {
            HttpTestHarness = harness;

            HttpTestHarness.OnConfigureHttpBus += ConfigureHttpBus;
            HttpTestHarness.OnConfigureHttpReceiveEndpoint += ConfigureHttpReceiveEndpoint;
        }

        protected HttpTestHarness HttpTestHarness { get; }

        protected Uri HostAddress => HttpTestHarness.HostAddress;

        protected ISendEndpoint RootEndpoint => HttpTestHarness.InputQueueSendEndpoint;

        [OneTimeSetUp]
        public Task SetupHttpTestFixture()
        {
            return HttpTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownHttpTestFixture()
        {
            return HttpTestHarness.Stop();
        }

        protected virtual void ConfigureHttpBus(IHttpBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureHttpBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
        {
        }

        protected virtual void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
        }
    }
}
