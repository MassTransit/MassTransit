namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using NUnit.Framework;


    public class GrpcClientTestFixture :
        GrpcTestFixture
    {
        BusHandle _clientBusHandle;
        IBusControl _clientBus;
        Uri _secondInputQueueAddress;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint ClientInputQueueSendEndpoint { get; set; }

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint ClientBusSendEndpoint { get; set; }

        protected Uri ClientBaseAddress { get; } = new Uri("http://127.0.0.1:19797/");

        protected Uri ClientInputQueueAddress
        {
            get => _secondInputQueueAddress;
            set
            {
                if (ClientBus != null)
                    throw new InvalidOperationException("The ClientBus has already been created, too late to change the URI");

                _secondInputQueueAddress = value;
            }
        }

        protected virtual IBus ClientBus => _clientBus;

        [OneTimeSetUp]
        public async Task SetupClientBus()
        {
            _clientBus = CreateClientBus();

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            _clientBusHandle = await _clientBus.StartAsync(timeout.Token);
            try
            {
                ClientBusSendEndpoint = await _clientBus.GetSendEndpoint(_clientBus.Address);

                ClientInputQueueSendEndpoint = await _clientBus.GetSendEndpoint(_secondInputQueueAddress);
            }
            catch (Exception)
            {
                try
                {
                    using var tokenSource = new CancellationTokenSource(TestTimeout);

                    await _clientBusHandle.StopAsync(tokenSource.Token);
                }
                finally
                {
                    _clientBusHandle = null;
                    _clientBus = null;
                }

                throw;
            }
        }

        [OneTimeTearDown]
        public async Task TearDownClientBus()
        {
            try
            {
                using var tokenSource = new CancellationTokenSource(TestTimeout);

                await _clientBusHandle?.StopAsync(tokenSource.Token);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "ClientBus Stop failed");
            }
            finally
            {
                _clientBusHandle = null;
                _clientBus = null;
            }
        }

        protected virtual void ConfigureClientBus(IGrpcBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureClientInputQueueEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateClientBus()
        {
            return MassTransit.Bus.Factory.CreateUsingGrpc(x =>
            {
                ConfigureClientBus(x);

                x.Host(ClientBaseAddress, h =>
                {
                    h.AddServer(BaseAddress);
                });

                x.ReceiveEndpoint("second-queue", e =>
                {
                    _secondInputQueueAddress = e.InputAddress;

                    ConfigureClientInputQueueEndpoint(e);
                });
            });
        }
    }
}
