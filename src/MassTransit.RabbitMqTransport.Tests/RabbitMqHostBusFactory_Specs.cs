// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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
            _settings = new TestRabbitMqSettings
            {
                Host = "localhost"
            };
        }

        [Test]
        public void Should_work_with_a_single_cluster_member()
        {
            _settings.ClusterMembers = "cluster1";

            _factory = new RabbitMqHostBusFactory(new TestSettingsProvider(_settings));

            var bus = _factory.CreateBus(new TestBusServiceConfiguartor(), "UnitTest");

            Assert.IsNotNull(bus);
        }

        [Test]
        public void Should_work_with_multiple_cluster_members()
        {
            _settings.ClusterMembers = "cluster1,cluster2,cluster3";

            _factory = new RabbitMqHostBusFactory(new TestSettingsProvider(_settings));

            var bus = _factory.CreateBus(new TestBusServiceConfiguartor(), "UnitTest");

            Assert.IsNotNull(bus);
        }

        [Test]
        public void Should_work_with_null()
        {
            _factory = new RabbitMqHostBusFactory(new TestSettingsProvider(_settings));

            var bus = _factory.CreateBus(new TestBusServiceConfiguartor(), "UnitTest");

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

        public bool TryGetSettings<T>(out T settings) where T : ISettings
        {
            throw new NotImplementedException();
        }

        public bool TryGetSettings<T>(string prefix, out T settings) where T : ISettings
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

    class TestBusServiceConfiguartor : IBusServiceConfigurator
    {
        public void Configure(IServiceConfigurator configurator)
        {
            // YAY!
        }
    }
}