namespace MassTransit.RabbitMqTransport.Tests
{
    using NUnit.Framework;
    using MassTransit.Hosting;
    using System;
    using Hosting;


    [TestFixture]
    public class WithSettings
    {
        RabbitMqHostBusFactory _factory;
        TestRabbitMqSettings _settings;

        [SetUp]
        public void Init()
        {
            _settings = new TestRabbitMqSettings {Host = "localhost"};
        }

        [Test]
        public void Should_work_with_a_single_cluster_member()
        {
            _settings.ClusterMembers = "cluster1";

            _factory = new RabbitMqHostBusFactory(new TestSettingsProvider(_settings));

            var bus = _factory.CreateBus(new TestBusServiceConfigurator(), "UnitTest");

            Assert.IsNotNull(bus);
        }

        [Test]
        public void Should_work_with_multiple_cluster_members()
        {
            _settings.ClusterMembers = "cluster1,cluster2,cluster3";

            _factory = new RabbitMqHostBusFactory(new TestSettingsProvider(_settings));

            var bus = _factory.CreateBus(new TestBusServiceConfigurator(), "UnitTest");

            Assert.IsNotNull(bus);
        }

        [Test]
        public void Should_work_with_null()
        {
            _factory = new RabbitMqHostBusFactory(new TestSettingsProvider(_settings));

            var bus = _factory.CreateBus(new TestBusServiceConfigurator(), "UnitTest");

            Assert.IsNotNull(bus);
        }
    }


    class TestSettingsProvider : ISettingsProvider
    {
        private RabbitMqSettings _settings;

        public TestSettingsProvider(RabbitMqSettings settings)
        {
            _settings = settings;
        }

        public bool TryGetSettings<T>(out T settings)
            where T : ISettings
        {
            throw new NotImplementedException();
        }

        public bool TryGetSettings<T>(string prefix, out T settings)
            where T : ISettings
        {
            settings = (T)_settings;

            return true;
        }
    }


    class TestRabbitMqSettings : RabbitMqSettings
    {
        public string ClusterMembers { get; set; }

        public ushort? Heartbeat { get; set; }

        public string Host { get; set; }

        public string Password { get; set; }

        public int? Port { get; set; }

        public string Username { get; set; }

        public string VirtualHost { get; set; }
    }


    class TestBusServiceConfigurator :
        IBusServiceConfigurator
    {
        public void Configure(IServiceConfigurator configurator)
        {
            // YAY!
        }
    }
}
