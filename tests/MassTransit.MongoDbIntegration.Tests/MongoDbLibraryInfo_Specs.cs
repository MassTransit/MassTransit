namespace MassTransit.MongoDbIntegration.Tests
{
    using MongoDB.Driver;
    using NUnit.Framework;


    [TestFixture]
    public class MongoClientSettingsExtensions_Specs
    {
        [Test]
        public void Should_set_library_info_from_connection_string()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb://127.0.0.1")
                .WithMassTransitLibraryInfo();
            Assert.That(settings.LibraryInfo, Is.Not.Null);
            Assert.That(settings.LibraryInfo.Name, Is.EqualTo("MassTransit"));
            Assert.That(settings.LibraryInfo.Version, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Should_set_library_info_from_mongo_url()
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://127.0.0.1"))
                .WithMassTransitLibraryInfo();
            Assert.That(settings.LibraryInfo, Is.Not.Null);
            Assert.That(settings.LibraryInfo.Name, Is.EqualTo("MassTransit"));
            Assert.That(settings.LibraryInfo.Version, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Should_return_same_settings_instance()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb://127.0.0.1");
            var result = settings.WithMassTransitLibraryInfo();
            Assert.That(result, Is.SameAs(settings));
        }
    }
}
