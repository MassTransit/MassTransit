namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using Azure.Core;
    using Azure.Storage;
    using Azure.Storage.Blobs;


    public class StorageSettings :
        IStorageSettings
    {
        public string ConnectionString { get; set; }
        public Uri ContainerUri { get; set; }
        public StorageSharedKeyCredential SharedKeyCredential { get; set; }
        public TokenCredential TokenCredential { get; set; }
        public Action<BlobClientOptions> Configure { get; set; }
    }
}
