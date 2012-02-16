namespace MassTransit.Tests.NewId_
{
    using NUnit.Framework;
    using NewIdProviders;

    [TestFixture]
    public class When_getting_a_network_address_for_the_id_generator
    {
        [Test]
        public void Should_pull_the_network_adapter_mac_address()
        {
            var networkIdProvider = new NetworkAddressWorkerIdProvider();

            byte[] networkId = networkIdProvider.GetWorkerId(0);

            Assert.IsNotNull(networkId);
            Assert.AreEqual(6, networkId.Length);
        }
    }
}