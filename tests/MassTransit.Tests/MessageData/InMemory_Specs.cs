namespace MassTransit.Tests.MessageData
{
    using System.IO;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using NUnit.Framework;


    public class InMemory_Specs
    {
        [TestFixture]
        public class Storing_message_data_in_memory
        {
            [Test]
            public async Task Should_set_and_get_data()
            {
                var data = new string('8', 10000);
                MessageData<string> property = await _repository.PutString(data);

                Assert.That(property.Address, Is.Not.Null);

                using var dataFromRepository = await _repository.Get(property.Address);

                using var reader = new StreamReader(dataFromRepository);
                var stringFromRepository = await reader.ReadToEndAsync();

                Assert.That(stringFromRepository, Is.EqualTo(data));
            }

            IMessageDataRepository _repository;

            [OneTimeSetUp]
            public void Setup()
            {
                _repository = new InMemoryMessageDataRepository();
            }
        }
    }
}
