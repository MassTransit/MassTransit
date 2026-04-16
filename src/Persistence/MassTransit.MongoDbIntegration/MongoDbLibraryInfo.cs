namespace MassTransit.MongoDbIntegration
{
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Configuration;


    static class MongoDbLibraryInfo
    {
        static readonly string _version =
            typeof(MongoDbLibraryInfo).Assembly.GetName().Version?.ToString() ?? "unknown";

        static readonly LibraryInfo _libraryInfo = new LibraryInfo("MassTransit", _version);

        internal static MongoClient CreateClient(string connectionString)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.LibraryInfo = _libraryInfo;
            return new MongoClient(settings);
        }

        internal static MongoClient CreateClient(MongoUrl mongoUrl)
        {
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.LibraryInfo = _libraryInfo;
            return new MongoClient(settings);
        }
    }
}
