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
            MaxDeliveryCount = 10;
        }

        public long? QueueId { get; set; }

        public int PrefetchCount => _configuration.Transport.PrefetchCount;

        public int ConcurrentMessageLimit => _configuration.Transport.GetConcurrentMessageLimit();

        public int ConcurrentDeliveryLimit
        {
            get => _concurrentDeliveryLimit;
            set
            {
                _concurrentDeliveryLimit = value;

                ReceiveMode = ReceiveMode switch
                {
                    SqlReceiveMode.Normal => ReceiveMode,
                    SqlReceiveMode.Partitioned when _concurrentDeliveryLimit > 1 => SqlReceiveMode.PartitionedConcurrent,
                    SqlReceiveMode.PartitionedOrdered when _concurrentDeliveryLimit > 1 => SqlReceiveMode.PartitionedOrderedConcurrent,
                    SqlReceiveMode.PartitionedConcurrent when _concurrentDeliveryLimit == 1 => SqlReceiveMode.Partitioned,
                    SqlReceiveMode.PartitionedOrderedConcurrent when _concurrentDeliveryLimit == 1 => SqlReceiveMode.PartitionedOrdered,
                    _ => ReceiveMode
                };
            }
        }

        public SqlReceiveMode ReceiveMode { get; set; }

        public bool PurgeOnStartup { get; set; }

        public TimeSpan LockDuration { get; set; }

        public int MaxDeliveryCount { get; set; }

        public TimeSpan PollingInterval { get; set; }

        public TimeSpan? UnlockDelay { get; set; }

        public TimeSpan MaxLockDuration { get; set; }

        public string EntityName => QueueName;

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
