namespace MassTransit.Tests.MessageData
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using NUnit.Framework;


    [TestFixture]
    public class Storing_message_data_on_the_file_system
    {
        [Test]
        public async Task Should_generate_the_folder_and_file()
        {
            MessageData<string> property = await _repository.PutString(new string('8', 10000));

            MessageData<string> loaded = await _repository.GetString(property.Address);
            await Assert.MultipleAsync(async () =>
            {
                Assert.That(property.Address, Is.Not.Null);


                Assert.That(await loaded.Value, Is.Not.Null);
            });
        }

        [Test]
        public async Task Should_generate_time_based_folder()
        {
            MessageData<string> property = await _repository.PutString(new string('8', 10000), TimeSpan.FromDays(30));

            MessageData<string> loaded = await _repository.GetString(property.Address);

            Assert.That(await loaded.Value, Is.Not.Null);
        }

        IMessageDataRepository _repository;

        [OneTimeSetUp]
        public void Setup()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var messageDataPath = Path.Combine(baseDirectory, "MessageData");

            var dataDirectory = new DirectoryInfo(messageDataPath);
            Console.WriteLine("Using data directory: {0}", dataDirectory);

            _repository = new FileSystemMessageDataRepository(dataDirectory);
        }
    }
}
