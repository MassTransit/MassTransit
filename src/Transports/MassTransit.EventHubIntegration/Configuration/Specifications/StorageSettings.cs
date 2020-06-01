namespace MassTransit.EventHubIntegration.Specifications
{
    using System;
    using Azure.Core;
    using Azure.Storage;
    using Azure.Storage.Blobs;


    public class StorageSettings :
        IStorageSettings
    {
        public StorageSettings(string connectionString, Action<BlobClientOptions> configure)
        {
            ConnectionString = connectionString;
            Configure = configure;
        }

        public StorageSettings(Uri containerUri, Action<BlobClientOptions> configure)
        {
            ContainerUri = containerUri;
            Configure = configure;
        }

        public StorageSettings(Uri containerUri, TokenCredential tokenCredential, Action<BlobClientOptions> configure)
        {
            ContainerUri = containerUri;
            TokenCredential = tokenCredential;
            Configure = configure;
        }

        public StorageSettings(Uri containerUri, StorageSharedKeyCredential sharedKeyCredential, Action<BlobClientOptions> configure)
        {
            ContainerUri = containerUri;
            SharedKeyCredential = sharedKeyCredential;
            Configure = configure;
        }

        public string ConnectionString { get; }
        public Uri ContainerUri { get; }
        public StorageSharedKeyCredential SharedKeyCredential { get; }
        public TokenCredential TokenCredential { get; }
        public Action<BlobClientOptions> Configure { get; }
    }
}
