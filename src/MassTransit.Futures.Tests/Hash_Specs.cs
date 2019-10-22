namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Text;
    using Conductor.Distribution;
    using Contracts;
    using NUnit.Framework;


    [TestFixture]
    public class An_empty_hash
    {
        [Test]
        public void Should_not_return_a_node()
        {
            var distribution = new ConsistentHashDistributionStrategy<string>(new Murmur3AUnsafeHashGenerator(), GetHashKey);
            distribution.Init(Enumerable.Empty<string>());

            var result = distribution.GetNode(Encoding.UTF8.GetBytes("Frank"));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Should_initialize_with_the_same_results()
        {
            EndpointInfo endpointA = new Endpoint() {EndpointId = NewId.NextGuid()};
            EndpointInfo endpointB = new Endpoint() {EndpointId = NewId.NextGuid()};
            EndpointInfo endpointC = new Endpoint() {EndpointId = NewId.NextGuid()};

            var serverA = new ConsistentHashDistributionStrategy<EndpointInfo>(new Murmur3AUnsafeHashGenerator(), x => x.EndpointId.ToByteArray());
            serverA.Init(Enumerable.Empty<EndpointInfo>());

            serverA.Add(endpointA);
            serverA.Add(endpointB);
            serverA.Add(endpointC);

            var serverB = new ConsistentHashDistributionStrategy<EndpointInfo>(new Murmur3AUnsafeHashGenerator(), x => x.EndpointId.ToByteArray());
            serverB.Init(Enumerable.Empty<EndpointInfo>());

            serverB.Add(endpointB);
            serverB.Add(endpointA);
            serverB.Add(endpointC);

            var serverC = new ConsistentHashDistributionStrategy<EndpointInfo>(new Murmur3AUnsafeHashGenerator(), x => x.EndpointId.ToByteArray());
            serverC.Init(Enumerable.Empty<EndpointInfo>());

            serverC.Add(endpointC);
            serverC.Add(endpointB);
            serverC.Add(endpointA);

            byte[] key = NewId.NextGuid().ToByteArray();

            var resultA = serverA.GetNode(key);
            var resultB = serverB.GetNode(key);
            var resultC = serverC.GetNode(key);

            Assert.That(resultA.EndpointId, Is.EqualTo(resultB.EndpointId));
            Assert.That(resultA.EndpointId, Is.EqualTo(resultC.EndpointId));

            key = Guid.NewGuid().ToByteArray();

            resultA = serverA.GetNode(key);
            resultB = serverB.GetNode(key);
            resultC = serverC.GetNode(key);

            Assert.That(resultA.EndpointId, Is.EqualTo(resultB.EndpointId));
            Assert.That(resultA.EndpointId, Is.EqualTo(resultC.EndpointId));
        }


        class Endpoint :
            EndpointInfo
        {
            public Guid EndpointId { get; set; }
            public DateTime Started { get; set; }
            public Uri InstanceAddress { get; set; }
            public Uri EndpointAddress { get; set; }
        }


        byte[] GetHashKey(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
