namespace MassTransit.MongoDbIntegration
{
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Configuration;


    public static class MongoClientSettingsExtensions
    {
        static readonly string _version =
            typeof(MongoClientSettingsExtensions).Assembly.GetName().Version?.ToString() ?? "unknown";

        static readonly LibraryInfo _libraryInfo = new LibraryInfo("MassTransit", _version);

        public static MongoClientSettings WithMassTransitLibraryInfo(this MongoClientSettings settings)
        {
            settings.LibraryInfo = _libraryInfo;
            return settings;
        }
    }
}
