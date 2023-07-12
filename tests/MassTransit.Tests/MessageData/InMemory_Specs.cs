namespace MassTransit.Tests.MessageData
{
    using System.IO;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using NUnit.Framework;

    [TestFixture]
    public class Storing_message_data_in_memory
    {
        [Test]
        public async Task Should_set_and_get_data()
        {
            var data = new string('8', 10000);
            MessageData<string> property = await _repository.PutString(data);

            Assert.That(property.Address, Is.Not.Null);

            var dataFromRepository = await _repository.Get(property.Address);

            using var reader = new StreamReader(dataFromRepository);
            var stringFromRepository = await reader.ReadToEndAsync();

            Assert.That(stringFromRepository, Is.EqualTo(data));
        }

        [Test]
        public async Task Should_set_and_delete_data()
        {
            var data = new string('8', 10000);
            MessageData<string> property = await _repository.PutString(data);

            Assert.That(property.Address, Is.Not.Null);

            var dataFromRepository = await _repository.Get(property.Address);

            using var reader = new StreamReader(dataFromRepository);
            var stringFromRepository = await reader.ReadToEndAsync();

            Assert.That(stringFromRepository, Is.EqualTo(data));

            await _repository.Delete(property.Address);

            Assert.ThrowsAsync<MessageDataNotFoundException>(async () => await _repository.Get(property.Address));
        }

        [Test]
        public async Task Should_not_throw_on_inline_data()
        {
            var data = new string('8', 256);
            MessageData<string> property = await _repository.PutString(data);

            Assert.That(property.Address, Is.Null);

            Assert.DoesNotThrowAsync(async () => await _repository.Delete(property.Address));
        }

        IMessageDataRepository _repository;

        [OneTimeSetUp]
        public void Setup()
        {
            MessageDataDefaults.AlwaysWriteToRepository = false;
            MessageDataDefaults.Threshold = 8194;
            _repository = new InMemoryMessageDataRepository();
        }
    }
}
