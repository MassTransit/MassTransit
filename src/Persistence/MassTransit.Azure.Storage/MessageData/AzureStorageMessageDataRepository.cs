namespace MassTransit.Azure.Storage.MessageData
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.MessageData;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Auth;
    using Microsoft.Azure.Storage.Blob;
    using Util;


    public class AzureStorageMessageDataRepository :
        IMessageDataRepository,
        IBusObserver
    {
        readonly StorageCredentials _credentials;
        readonly IBlobNameGenerator _nameGenerator;
        readonly Uri _containerUri;

        public AzureStorageMessageDataRepository(Uri storageEndpoint, string containerName, StorageCredentials credentials, IBlobNameGenerator nameGenerator)
        {
            _credentials = credentials;
            _nameGenerator = nameGenerator;

            var containerUriBase = $"{storageEndpoint}".TrimEnd('/');

            _containerUri = new Uri($"{containerUriBase}/{containerName}");
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = default)
        {
            var blob = new CloudBlockBlob(address, _credentials);
            try
            {
                return await blob.OpenReadAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (StorageException exception)
            {
                throw new MessageDataException($"MessageData content not found: {blob.Container.Name}/{blob.Name}", exception);
            }
        }

        public async Task<Uri> Put(Stream stream, TimeSpan? timeToLive = default, CancellationToken cancellationToken = default)
        {
            var blobName = _nameGenerator.GenerateBlobName();
            var blobAddress = new Uri($"{_containerUri}/{blobName}");
            var blob = new CloudBlockBlob(blobAddress, _credentials);

            SetBlobExpiration(blob, timeToLive);

            await blob.UploadFromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("MessageData:Put {Blob} {Address}", blob.Name, blob.Uri);

            return blob.Uri;
        }

        static void SetBlobExpiration(CloudBlockBlob blob, TimeSpan? timeToLive)
        {
            if (timeToLive.HasValue)
            {
                var utcNow = DateTime.UtcNow;

                var expirationDate = utcNow + timeToLive.Value;
                if (expirationDate <= utcNow)
                    expirationDate = utcNow + TimeSpan.FromMinutes(1);

                blob.Metadata["ValidUntilUtc"] = expirationDate.ToString("O");
            }
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
            var container = new CloudBlobContainer(_containerUri, _credentials);

            var containerExists = await container.ExistsAsync().ConfigureAwait(false);
            if (!containerExists)
            {
                try
                {
                    await container.CreateIfNotExistsAsync().ConfigureAwait(false);
                }
                catch (StorageException exception)
                {
                    LogContext.Warning?.Log(exception, "Azure Storage Container does not exist: {Address}", _containerUri);
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
    }
}