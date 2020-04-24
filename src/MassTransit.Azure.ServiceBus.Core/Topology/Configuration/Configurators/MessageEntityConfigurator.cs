namespace MassTransit.Azure.ServiceBus.Core.Topology.Configurators
{
    using System;


    public abstract class MessageEntityConfigurator :
        EntityConfigurator,
        IMessageEntityConfigurator
    {
        string _basePath;

        protected MessageEntityConfigurator(string path)
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

        public long? MaxSizeInMB { get; set; }

        public bool? RequiresDuplicateDetection { get; set; }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            RequiresDuplicateDetection = true;
            DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }
    }
}
