namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Registration;
    using TestFramework;


    public abstract class KafkaTestFixture :
        InMemoryTestFixture
    {
        readonly CancellationTokenSource _cancellationTokenSource;

        protected KafkaTestFixture(TimeSpan? timeout = null)
        {
            _cancellationTokenSource = new CancellationTokenSource(timeout ?? TimeSpan.FromMinutes(1));
        }

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected abstract IBusInstance BusInstance { get; }

        protected virtual void ConfigureBusAttachment<T>(IBusAttachmentRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.UsingKafka(ConfigureKafka);
        }

        protected virtual void ConfigureKafka<T>(IBusAttachmentRegistrationContext<T> context, IKafkaFactoryConfigurator configurator)
            where T : class
        {
            configurator.Host("localhost:9092");
        }

        [OneTimeSetUp]
        public async Task Start()
        {
            await BusInstance.Start(CancellationToken);
        }

        [OneTimeTearDown]
        public async Task Stop()
        {
            await BusInstance.Stop(CancellationToken);
            _cancellationTokenSource.Dispose();
        }
    }
}
