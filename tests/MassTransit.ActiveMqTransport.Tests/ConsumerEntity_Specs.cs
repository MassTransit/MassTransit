namespace MassTransit.ActiveMqTransport.Tests
{
    using System.Collections;
    using NUnit.Framework;
    using Topology;


    [TestFixture]
    public class ConsumerEntity_Specs
    {
        [Test]
        [TestCaseSource(nameof(CreateCasesForEntityComparerEquals))]
        public bool ConsumerEntityComparer_Equal_should_return_expected(ConsumerEntity left, ConsumerEntity right)
        {
            // Act
            return ConsumerEntity.EntityComparer.Equals(left, right);
        }

        [Test]
        [TestCaseSource(nameof(CreateCasesForNameComparerEquals))]
        public bool NameComparer_Equal_should_return_expected(ConsumerEntity left, ConsumerEntity right)
        {
            // Act
            return ConsumerEntity.NameComparer.Equals(left, right);
        }

        public static IEnumerable CreateCasesForEntityComparerEquals()
        {
            var topic = new TopicEntity(1, "topic", false, true);
            var queue = new QueueEntity(1, "queue", false, true);

            yield return new TestCaseData(new ConsumerEntity(1, topic, queue, "selector"), new ConsumerEntity(1, topic, queue, "selector")).Returns(true);
            yield return new TestCaseData(new ConsumerEntity(1, topic, queue, "selector"), null).Returns(false);
            yield return new TestCaseData(null, new ConsumerEntity(1, topic, queue, "selector")).Returns(false);
            yield return new TestCaseData(new ConsumerEntity(1, topic, null, "selector"), new ConsumerEntity(1, topic, queue, "selector")).Returns(false);
            yield return new TestCaseData(new ConsumerEntity(1, topic, queue, "selector"), new ConsumerEntity(1, topic, null, "selector")).Returns(false);
            yield return new TestCaseData(new ConsumerEntity(1, topic, null, "selector"), new ConsumerEntity(1, topic, null, "selector")).Returns(true);
            yield return new TestCaseData(null, null).Returns(true);
        }

        public static IEnumerable CreateCasesForNameComparerEquals()
        {
            var leftTopic = new TopicEntity(1, "topic", false, true);
            var leftQueue = new QueueEntity(1, "queue", false, true);
            var rightTopic = new TopicEntity(1, "topic", false, true);
            var rightQueue = new QueueEntity(1, "queue", false, true);

            yield return new TestCaseData(new ConsumerEntity(1, leftTopic, leftQueue, "selector"), new ConsumerEntity(1, rightTopic, rightQueue, "selector"))
                .Returns(true);
            yield return new TestCaseData(new ConsumerEntity(1, leftTopic, leftQueue, "selector"), null).Returns(false);
            yield return new TestCaseData(null, new ConsumerEntity(1, rightTopic, rightQueue, "selector")).Returns(false);
            yield return new TestCaseData(new ConsumerEntity(1, leftTopic, null, "selector"), new ConsumerEntity(1, rightTopic, rightQueue, "selector"))
                .Returns(false);
            yield return new TestCaseData(new ConsumerEntity(1, leftTopic, leftQueue, "selector"), new ConsumerEntity(1, rightTopic, null, "selector"))
                .Returns(false);
            yield return new TestCaseData(new ConsumerEntity(1, leftTopic, null, "selector"), new ConsumerEntity(1, rightTopic, null, "selector"))
                .Returns(true);
            yield return new TestCaseData(null, null).Returns(true);
        }
    }
}
