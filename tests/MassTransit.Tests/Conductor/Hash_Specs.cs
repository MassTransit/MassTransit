namespace MassTransit.Tests.Conductor
{
    using System;
    using System.Linq;
    using System.Text;
    using Contracts;
    using MassTransit.Conductor.Distribution;
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
            InstanceInfo nodeA = new Node() {InstanceId = NewId.NextGuid()};
            InstanceInfo nodeB = new Node() {InstanceId = NewId.NextGuid()};
            InstanceInfo nodeC = new Node() {InstanceId = NewId.NextGuid()};

            var serverA = new ConsistentHashDistributionStrategy<InstanceInfo>(new Murmur3AUnsafeHashGenerator(), x => x.InstanceId.ToByteArray());
            serverA.Init(Enumerable.Empty<InstanceInfo>());

            serverA.Add(nodeA);
            serverA.Add(nodeB);
            serverA.Add(nodeC);

            var serverB = new ConsistentHashDistributionStrategy<InstanceInfo>(new Murmur3AUnsafeHashGenerator(), x => x.InstanceId.ToByteArray());
            serverB.Init(Enumerable.Empty<InstanceInfo>());

            serverB.Add(nodeB);
            serverB.Add(nodeA);
            serverB.Add(nodeC);

            var serverC = new ConsistentHashDistributionStrategy<InstanceInfo>(new Murmur3AUnsafeHashGenerator(), x => x.InstanceId.ToByteArray());
            serverC.Init(Enumerable.Empty<InstanceInfo>());

            serverC.Add(nodeC);
            serverC.Add(nodeB);
            serverC.Add(nodeA);

            byte[] key = NewId.NextGuid().ToByteArray();

            var resultA = serverA.GetNode(key);
            var resultB = serverB.GetNode(key);
            var resultC = serverC.GetNode(key);

            Assert.That(resultA.InstanceId, Is.EqualTo(resultB.InstanceId));
            Assert.That(resultA.InstanceId, Is.EqualTo(resultC.InstanceId));

            key = Guid.NewGuid().ToByteArray();

            resultA = serverA.GetNode(key);
            resultB = serverB.GetNode(key);
            resultC = serverC.GetNode(key);

            Assert.That(resultA.InstanceId, Is.EqualTo(resultB.InstanceId));
            Assert.That(resultA.InstanceId, Is.EqualTo(resultC.InstanceId));
        }


        class Node :
            InstanceInfo
        {
            public Guid InstanceId { get; set; }
            public DateTime? Started { get; set; }
            public ServiceCapability[] Capabilities { get; set; }
            public Uri Address { get; set; }
            public Uri ServiceAddress { get; set; }
        }


        byte[] GetHashKey(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
