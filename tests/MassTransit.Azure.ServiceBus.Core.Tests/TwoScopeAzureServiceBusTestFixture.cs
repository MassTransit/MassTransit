namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using NUnit.Framework;


    public class TwoScopeAzureServiceBusTestFixture :
        AzureServiceBusTestFixture
    {
        public TwoScopeAzureServiceBusTestFixture(string scope)
        {
            _secondServiceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace, scope);
        }

        public TwoScopeAzureServiceBusTestFixture()
        {
            _secondServiceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace, "MassTransit.Tests.SecondService");
        }

        Uri _secondInputQueueAddress;
        readonly Uri _secondServiceUri;
        IBusControl _secondBus;
        BusHandle _secondBusHandle;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint SecondInputQueueSendEndpoint { get; set; }

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint SecondBusSendEndpoint { get; set; }

        protected Uri SecondBusAddress => _secondBus.Address;

        protected Uri SecondInputQueueAddress
        {
            get => _secondInputQueueAddress;
            set
            {
                if (SecondBus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _secondInputQueueAddress = value;
            }
        }

        protected virtual IBus SecondBus => _secondBus;

        [OneTimeSetUp]
        public async Task SetupSecondAzureServiceBusTestFixture()
        {
            _secondBus = CreateSecondBus();

            _secondBusHandle = await _secondBus.StartAsync();
            try
            {
                SecondBusSendEndpoint = await _secondBus.GetSendEndpoint(_secondBus.Address);

                SecondInputQueueSendEndpoint = await _secondBus.GetSendEndpoint(_secondInputQueueAddress);
            }
            catch (Exception)
            {
                try
                {
                    using (var tokenSource = new CancellationTokenSource(TestTimeout))
                    {
                        await _secondBusHandle.StopAsync(tokenSource.Token);
                    }
                }
                finally
                {
                    _secondBusHandle = null;
                    _secondBus = null;
                }

                throw;
            }
        }

        [OneTimeTearDown]
        public async Task TearDownTwoScopeTestFixture()
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(TestTimeout))
                {
                    await _secondBusHandle?.StopAsync(tokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "SecondBus Stop failed");
            }
            finally
            {
                _secondBusHandle = null;
                _secondBus = null;
            }
        }

        protected virtual void ConfigureSecondBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureSecondInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateSecondBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                ConfigureSecondBus(x);

                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                x.Host(_secondServiceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                x.ReceiveEndpoint("input_queue", e =>
                {
                    _secondInputQueueAddress = e.InputAddress;

                    ConfigureSecondInputQueueEndpoint(e);
                });
            });
        }
    }
}
