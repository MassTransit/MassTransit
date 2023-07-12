namespace MassTransit.Azure.Storage.Tests
{
    using System;
    using System.Threading.Tasks;
    using TestFramework;
    using NUnit.Framework;
    using System.IO;
    using MassTransit.AzureStorage.MessageData;

    public static class Configuration
    {
        public static string StorageAccount =>
          Environment.GetEnvironmentVariable("MT_AZURE_STORAGE_ACCOUNT")
            ?? Environment.GetEnvironmentVariable("AZURE_STORAGE")
            ?? "DefaultEndpointsProtocol=http;AccountName=localhost;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;TableEndpoint=https://localhost:8081/;";
    }


    public class Azure_Storage_Tests :
        InMemoryTestFixture
    {
        protected readonly string ConnectionString;
        private AzureStorageMessageDataRepository _repository;

        public Azure_Storage_Tests()
        {
            ConnectionString = Configuration.StorageAccount;
        }

         [Test]
        public async Task Should_generate_the_folder_and_file()
        {
            MessageData<string> property = await _repository.PutString(new string('8', 10000));

            MessageData<string> loaded = await _repository.GetString(property.Address);

            Assert.That(property.Address, Is.Not.Null);

            Assert.That(await loaded.Value, Is.Not.Null);
        }

        //[Test]
        //public async Task Should_generate_time_based_folder()
        //{
        //    MessageData<string> property = await _repository.PutString(new string('8', 10000), TimeSpan.FromDays(30));

        //    MessageData<string> loaded = await _repository.GetString(property.Address);

        //    Assert.That(await loaded.Value, Is.Not.Null);
        //}

        [Test]
        public async Task Should_set_and_delete_data()
        {
            var data = new string('8', 10000);
            MessageData<string> property = await _repository.PutString(data);

            Assert.That(property.Address, Is.Not.Null);

            var dataFromRepository = await _repository.Get(property.Address);

            {
                using var reader = new StreamReader(dataFromRepository);
                var stringFromRepository = await reader.ReadToEndAsync();

                Assert.That(stringFromRepository, Is.EqualTo(data));
            }

            await _repository.Delete(property.Address);

            Assert.ThrowsAsync<MessageDataNotFoundException>(async () => await _repository.Get(property.Address));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);
        }

        [OneTimeSetUp]
        public async Task Bring_it_up()
        {
            MessageDataDefaults.AlwaysWriteToRepository = false;
            MessageDataDefaults.Threshold = 8194;


            _repository = new AzureStorageMessageDataRepository(ConnectionString, "MessageData");
        }


    }
}

//namespace MassTransit.Tests.MessageData
//{
//    using System;
//    using System.IO;
//    using System.Threading.Tasks;
//    using MassTransit.MessageData;
//    using NUnit.Framework;


//    [TestFixture]
//    public class Storing_message_data_on_the_file_system
//    {
//       

//        [Test]
//        public async Task Should_not_throw_on_inline_data()
//        {
//            var data = new string('8', 256);
//            MessageData<string> property = await _repository.PutString(data);

//            Assert.That(property.Address, Is.Null);

//            Assert.DoesNotThrowAsync(async () => await _repository.Delete(property.Address));
//        }

//        IMessageDataRepository _repository;

//        [OneTimeSetUp]
//        public void Setup()
//        {
//            MessageDataDefaults.AlwaysWriteToRepository = false;
//            MessageDataDefaults.Threshold = 8194;

//            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
//            var messageDataPath = Path.Combine(baseDirectory, "MessageData");

//            var dataDirectory = new DirectoryInfo(messageDataPath);
//            Console.WriteLine("Using data directory: {0}", dataDirectory);

//            _repository = new FileSystemMessageDataRepository(dataDirectory);
//        }
//    }
//}
