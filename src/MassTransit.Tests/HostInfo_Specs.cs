// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Host_info_should_be_included_on_json_serialization :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_match_the_sending_host_information()
        {
            await Bus.Publish(new PingMessage());

            var context = await _handled;

            Assert.IsNotNull(context.Host);

            Assert.AreEqual(HostMetadataCache.Host.MachineName, context.Host.MachineName);
            Assert.AreEqual(HostMetadataCache.Host.Assembly, context.Host.Assembly);
            Assert.AreEqual(HostMetadataCache.Host.AssemblyVersion, context.Host.AssemblyVersion);
            Assert.AreEqual(HostMetadataCache.Host.FrameworkVersion, context.Host.FrameworkVersion);
            Assert.AreEqual(HostMetadataCache.Host.MassTransitVersion, context.Host.MassTransitVersion);
            Assert.AreEqual(HostMetadataCache.Host.OperatingSystemVersion, context.Host.OperatingSystemVersion);
            Assert.AreEqual(HostMetadataCache.Host.ProcessName, context.Host.ProcessName);
            Assert.AreEqual(HostMetadataCache.Host.ProcessId, context.Host.ProcessId);
        }
    }

#if !NETCORE

    [TestFixture]
    public class Host_info_should_be_included_on_binary_serialization :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseBinarySerializer();

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_match_the_sending_host_information()
        {
            await Bus.Publish(new PingMessage());

            var context = await _handled;

            Assert.IsNotNull(context.Host);

            Assert.AreEqual(HostMetadataCache.Host.MachineName, context.Host.MachineName);
            Assert.AreEqual(HostMetadataCache.Host.Assembly, context.Host.Assembly);
            Assert.AreEqual(HostMetadataCache.Host.AssemblyVersion, context.Host.AssemblyVersion);
            Assert.AreEqual(HostMetadataCache.Host.FrameworkVersion, context.Host.FrameworkVersion);
            Assert.AreEqual(HostMetadataCache.Host.MassTransitVersion, context.Host.MassTransitVersion);
            Assert.AreEqual(HostMetadataCache.Host.OperatingSystemVersion, context.Host.OperatingSystemVersion);
            Assert.AreEqual(HostMetadataCache.Host.ProcessName, context.Host.ProcessName);
            Assert.AreEqual(HostMetadataCache.Host.ProcessId, context.Host.ProcessId);
        }
    }

#endif
}