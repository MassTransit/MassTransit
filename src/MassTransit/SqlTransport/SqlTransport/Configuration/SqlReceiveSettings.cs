namespace MassTransit.SqlTransport.Configuration
{
    using System;


    public class SqlReceiveSettings :
        SqlQueueConfigurator,
        ReceiveSettings
    {
        readonly ISqlEndpointConfiguration _configuration;
        int _concurrentDeliveryLimit;

        public SqlReceiveSettings(ISqlEndpointConfiguration configuration, string queueName, TimeSpan? autoDeleteOnIdle = null)
            : base(queueName, autoDeleteOnIdle)
        {
            _configuration = configuration;

            PollingInterval = TimeSpan.FromSeconds(1);
            ConcurrentDeliveryLimit = 1;

            LockDuration = TimeSpan.FromMinutes(1);
            MaxLockDuration = TimeSpan.FromHours(12);
            MaintenanceBatchSize = 100;
        }

        public long? QueueId { get; set; }

        public int PrefetchCount => _configuration.Transport.PrefetchCount;

        public int ConcurrentMessageLimit => _configuration.Transport.GetConcurrentMessageLimit();

        public int ConcurrentDeliveryLimit
        {
            get
            {
                return ReceiveMode switch
                {
                    SqlReceiveMode.Normal => 1,
                    SqlReceiveMode.Partitioned => 1,
                    SqlReceiveMode.PartitionedOrdered => 1,
                    _ => _concurrentDeliveryLimit
                };
            }
            set => _concurrentDeliveryLimit = value;
        }

        public SqlReceiveMode ReceiveMode { get; set; }

        public bool PurgeOnStartup { get; set; }

        public TimeSpan LockDuration { get; set; }

        public TimeSpan PollingInterval { get; set; }

        public TimeSpan? UnlockDelay { get; set; }

        public TimeSpan MaxLockDuration { get; set; }

        public string EntityName => QueueName;

        public int MaintenanceBatchSize { get; set; }

        public bool DeadLetterExpiredMessages { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
