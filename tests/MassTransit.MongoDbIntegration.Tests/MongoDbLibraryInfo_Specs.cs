namespace MassTransit.MongoDbIntegration.Tests
{
    using MongoDB.Driver;
    using NUnit.Framework;


    [TestFixture]
    public class MongoDbLibraryInfo_Specs
    {
        [Test]
        public void Should_set_library_info_on_connection_string_client()
        {
            var client = MongoDbLibraryInfo.CreateClient("mongodb://127.0.0.1");
            Assert.That(client.Settings.LibraryInfo, Is.Not.Null);
            Assert.That(client.Settings.LibraryInfo.Name, Is.EqualTo("MassTransit"));
            Assert.That(client.Settings.LibraryInfo.Version, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Should_set_library_info_on_mongo_url_client()
        {
            var client = MongoDbLibraryInfo.CreateClient(new MongoUrl("mongodb://127.0.0.1"));
            Assert.That(client.Settings.LibraryInfo, Is.Not.Null);
            Assert.That(client.Settings.LibraryInfo.Name, Is.EqualTo("MassTransit"));
            Assert.That(client.Settings.LibraryInfo.Version, Is.Not.Null.And.Not.Empty);
        }
    }
}
