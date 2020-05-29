namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_that_implements_an_interface :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message_to_an_both_interested_consumers()
        {
            Task<ConsumeContext<FirstMessageContract>> first = SubscribeHandler<FirstMessageContract>();
            Task<ConsumeContext<SecondMessageContract>> second = SubscribeHandler<SecondMessageContract>();

            var message = new SomeMessageContract("Joe", 27);

            await BusSendEndpoint.Send(message);

            await BusSendEndpoint.Send(message);

            await first;
            await second;
        }

        [Test]
        public async Task Should_deliver_the_message_to_an_interested_consumer()
        {
            Task<ConsumeContext<FirstMessageContract>> first = SubscribeHandler<FirstMessageContract>();

            var message = new SomeMessageContract("Joe", 27);

            await BusSendEndpoint.Send(message);

            await first;
        }


        public interface FirstMessageContract
        {
            string Name { get; }
        }


        public interface SecondMessageContract
        {
            string Name { get; }
            int Age { get; }
        }


        class SomeMessageContract :
            FirstMessageContract,
            SecondMessageContract
        {
            public SomeMessageContract(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; private set; }
            public int Age { get; private set; }
        }
    }
}
