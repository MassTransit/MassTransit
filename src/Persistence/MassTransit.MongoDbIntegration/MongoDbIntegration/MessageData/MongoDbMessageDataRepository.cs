namespace MassTransit.MongoDbIntegration.MessageData
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;


    public class MongoDbMessageDataRepository :
        IMessageDataRepository
    {
        readonly IGridFSBucket _bucket;
        readonly IFileNameGenerator _fileNameGenerator;
        readonly IMessageDataResolver _resolver;

        public MongoDbMessageDataRepository(string connectionString, string database)
            : this(new MessageDataResolver(), new GridFSBucket(new MongoClient(connectionString).GetDatabase(database)), new NewIdFileNameGenerator())
        {
        }

        public MongoDbMessageDataRepository(MongoUrl mongoUrl)
            : this(mongoUrl.Url, mongoUrl.DatabaseName)
        {
        }

        public MongoDbMessageDataRepository(IMessageDataResolver resolver, IGridFSBucket bucket, IFileNameGenerator fileNameGenerator)
        {
            _resolver = resolver;
            _bucket = bucket;
            _fileNameGenerator = fileNameGenerator;
        }

        async Task<Stream> IMessageDataRepository.Get(Uri address, CancellationToken cancellationToken)
        {
            var id = _resolver.GetObjectId(address);

            return await _bucket.OpenDownloadStreamAsync(id, null, cancellationToken).ConfigureAwait(false);
        }

        async Task<Uri> IMessageDataRepository.Put(Stream stream, TimeSpan? timeToLive, CancellationToken cancellationToken)
        {
            var options = BuildGridFsUploadOptions(timeToLive);

            var filename = _fileNameGenerator.GenerateFileName();

            var id = await _bucket.UploadFromStreamAsync(filename, stream, options, cancellationToken)
                .ConfigureAwait(false);

            LogContext.Debug?.Log("MessageData:Put {Id} {FileName}", id, filename);

            return _resolver.GetAddress(id);
        }

        GridFSUploadOptions BuildGridFsUploadOptions(TimeSpan? timeToLive)
        {
            var metadata = new BsonDocument();

            if (timeToLive.HasValue)
                metadata["expiration"] = DateTime.UtcNow.Add(timeToLive.Value);

            return new GridFSUploadOptions { Metadata = metadata };
        }
    }
}
