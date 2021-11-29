namespace MassTransit.Abstractions.Tests
{
    using System;
    using System.Linq;
    using System.Net.NetworkInformation;
    using NewIdProviders;
    using NUnit.Framework;


    [TestFixture]
    public class When_getting_a_network_address_for_the_id_generator
    {
        [Test]
        public void Should_pull_all_adapters()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .ToArray();

            foreach (var networkInterface in interfaces)
            {
                Console.WriteLine("Network Interface: {0} - {1}", networkInterface.Description,
                    networkInterface.NetworkInterfaceType);
            }
        }

        [Test]
        public void Should_pull_the_network_adapter_mac_address()
        {
            var networkIdProvider = new NetworkAddressWorkerIdProvider();

            var networkId = networkIdProvider.GetWorkerId(0);

            Assert.IsNotNull(networkId);
            Assert.AreEqual(6, networkId.Length);
        }

        [Test]
        public void Should_pull_using_host_name()
        {
            var networkIdProvider = new HostNameHashWorkerIdProvider();

            var networkId = networkIdProvider.GetWorkerId(0);

            Assert.IsNotNull(networkId);
            Assert.AreEqual(6, networkId.Length);
        }

        [Test]
        public void Should_pull_using_net()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    || x.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet
                    || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx
                    || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT
                ).ToArray();

            foreach (var networkInterface in interfaces)
                Console.WriteLine("Network Interface: {0}", networkInterface.Description);
        }
    }
}
