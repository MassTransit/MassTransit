// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.MongoDbIntegration.MessageData
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.MessageData;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;


    public class MongoDbMessageDataRepository :
        IMessageDataRepository
    {
        readonly IGridFSBucket _bucket;
        readonly IFileNameGenerator _fileNameGenerator;
        readonly ILogger _logger = Logger.Get<MongoDbMessageDataRepository>();
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

            _logger.LogDebug("MessageData:Put {0} - {1}", id, filename);

            return _resolver.GetAddress(id);
        }

        GridFSUploadOptions BuildGridFsUploadOptions(TimeSpan? timeToLive)
        {
            var metadata = new BsonDocument();

            if (timeToLive.HasValue)
            {
                metadata["expiration"] = DateTime.UtcNow.Add(timeToLive.Value);
            }

            return new GridFSUploadOptions {Metadata = metadata};
        }
    }
}
