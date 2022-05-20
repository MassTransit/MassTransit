namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;


    public abstract class ServiceBusMessageEntityConfigurator :
        ServiceBusEntityConfigurator,
        IServiceBusMessageEntityConfigurator
    {
        string _basePath;

        protected ServiceBusMessageEntityConfigurator(string path)
        {
            Path = path;

            AutoDeleteOnIdle = Defaults.AutoDeleteOnIdle;
            DefaultMessageTimeToLive = Defaults.DefaultMessageTimeToLive;
            EnableBatchedOperations = true;
        }

        public string Path { get; set; }

        public string BasePath
        {
            get => _basePath;
            set => _basePath = value?.Trim('/');
        }

        public string FullPath => string.IsNullOrEmpty(BasePath) ? Path : $"{BasePath}/{Path.Trim('/')}";

        public TimeSpan? DuplicateDetectionHistoryTimeWindow { get; set; }

        public bool? EnablePartitioning { get; set; }

        public long? MaxSizeInMegabytes { get; set; }

        public long? MaxMessageSizeInKilobytes { get; set; }

        public bool? RequiresDuplicateDetection { get; set; }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            RequiresDuplicateDetection = true;
            DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }
    }
}
