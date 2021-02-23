namespace MassTransit.Azure.Storage.MessageData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.MessageData;
    using global::Azure;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;
    using Util;

    public class AzureStorageMessageDataRepository :
        IMessageDataRepository,
        IBusObserver
    {
        private readonly BlobContainerClient _container;
        readonly IBlobNameGenerator _nameGenerator;

        public AzureStorageMessageDataRepository(BlobServiceClient blobServiceClient, string containerName, IBlobNameGenerator nameGenerator)
        {
            _container = blobServiceClient.GetBlobContainerClient(containerName);
            _nameGenerator = nameGenerator;
        }

        public Task PostCreate(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task PreStart(IBus bus)
        {
            var containerExists = await _container.ExistsAsync().ConfigureAwait(false);
            if (!containerExists)
            {
                try
                {
                    await _container.CreateIfNotExistsAsync().ConfigureAwait(false);
                }
                catch (RequestFailedException exception)
                {
                    LogContext.Warning?.Log(exception, "Azure Storage Container does not exist: {Address}", _container.Uri);
                }
            }
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return TaskUtil.Completed;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = default)
        {
            var blobName = new BlobUriBuilder(address).BlobName;
            var blob = _container.GetBlobClient(blobName);
            try
            {
                return await blob.OpenReadAsync(new BlobOpenReadOptions(false), cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException exception)
            {
                throw new MessageDataException($"MessageData content not found: {blob.BlobContainerName}/{blob.Name}", exception);
            }
        }

        public async Task<Uri> Put(Stream stream, TimeSpan? timeToLive = default, CancellationToken cancellationToken = default)
        {
            var blobName = _nameGenerator.GenerateBlobName();
            var blob = _container.GetBlobClient(blobName);

            await blob.UploadAsync(stream, cancellationToken).ConfigureAwait(false);

            await SetBlobExpiration(blob, timeToLive).ConfigureAwait(false);

            LogContext.Debug?.Log("MessageData:Put {Blob} {Address}", blob.Name, blob.Uri);

            return blob.Uri;
        }

        static async Task SetBlobExpiration(BlobClient blob, TimeSpan? timeToLive)
        {
            if (timeToLive.HasValue)
            {
                var utcNow = DateTime.UtcNow;

                var expirationDate = utcNow + timeToLive.Value;
                if (expirationDate <= utcNow)
                    expirationDate = utcNow + TimeSpan.FromMinutes(1);

                var metadata = new Dictionary<string, string> { { "ValidUntilUtc", expirationDate.ToString("O") } };
                await blob.SetMetadataAsync(metadata).ConfigureAwait(false);
            }
        }
    }
}
