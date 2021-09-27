using AzureSasCredential = Azure.AzureSasCredential;
using BlobBaseClient = Azure.Storage.Blobs.Specialized.BlobBaseClient;
using BlobContainerClient = Azure.Storage.Blobs.BlobContainerClient;
using BlobOpenReadOptions = Azure.Storage.Blobs.Models.BlobOpenReadOptions;
using BlobServiceClient = Azure.Storage.Blobs.BlobServiceClient;
using BlobUriBuilder = Azure.Storage.Blobs.BlobUriBuilder;
using ClientSecretCredential = Azure.Identity.ClientSecretCredential;
using RequestFailedException = Azure.RequestFailedException;
using StorageSharedKeyCredential = Azure.Storage.StorageSharedKeyCredential;


namespace MassTransit.Azure.Storage.MessageData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.MessageData;
    using Util;


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
            try
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
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Azure Storage failure.");
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
