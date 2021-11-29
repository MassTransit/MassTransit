namespace MassTransit.AzureStorage.MessageData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Identity;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;


    public class AzureStorageMessageDataRepository :
        IMessageDataRepository,
        IBusObserver
    {
        readonly BlobContainerClient _container;
        readonly IBlobNameGenerator _nameGenerator;

        public AzureStorageMessageDataRepository(string connectionString, string containerName)
            : this(new BlobServiceClient(connectionString), containerName)
        {
        }

        public AzureStorageMessageDataRepository(Uri serviceUri, string containerName, string accountName, string accountKey)
            : this(new BlobServiceClient(serviceUri, new StorageSharedKeyCredential(accountName, accountKey)), containerName)
        {
        }

        public AzureStorageMessageDataRepository(Uri serviceUri, string containerName, string signature)
            : this(new BlobServiceClient(serviceUri, new AzureSasCredential(signature)), containerName)
        {
        }

        public AzureStorageMessageDataRepository(Uri serviceUri, string containerName, string tenantId, string clientId, string clientSecret)
            : this(new BlobServiceClient(serviceUri, new ClientSecretCredential(tenantId, clientId, clientSecret)), containerName)
        {
        }

        public AzureStorageMessageDataRepository(BlobServiceClient client, string containerName)
            : this(client, containerName, new NewIdBlobNameGenerator())
        {
        }

        public AzureStorageMessageDataRepository(BlobServiceClient client, string containerName, IBlobNameGenerator nameGenerator)
        {
            _container = client.GetBlobContainerClient(containerName);
            _nameGenerator = nameGenerator;
        }

        public void PostCreate(IBus bus)
        {
        }

        public void CreateFaulted(Exception exception)
        {
        }

        public async Task PreStart(IBus bus)
        {
            try
            {
                Response<bool> containerExists = await _container.ExistsAsync().ConfigureAwait(false);
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
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Azure Storage failure.");
            }
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = default)
        {
            var blobName = new BlobUriBuilder(address).BlobName;
            var blob = _container.GetBlobClient(blobName);
            try
            {
                LogContext.Debug?.Log("GET Message Data: {Address} ({Blob})", address, blob.Name);

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

            LogContext.Debug?.Log("PUT Message Data: {Address} ({Blob})", blob.Uri, blob.Name);

            return blob.Uri;
        }

        static async Task SetBlobExpiration(BlobBaseClient blob, TimeSpan? timeToLive)
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
