namespace MassTransit.Transports.Fabric
{
    public class QueueMetric
    {
        public QueueMetric(string name)
        {
            Name = name;
            DeliveryCount = new Counter();
            ActiveDeliveryCount = new Gauge();
            DelayedMessageCount = new Gauge();
            MessageCount = new Gauge();
        }

        public string Name { get; }

        /// <summary>
        /// Total number of messages delivered
        /// </summary>
        public Counter DeliveryCount { get; }

        /// <summary>
        /// Number of messages currently being delivered
        /// </summary>
        public Gauge ActiveDeliveryCount { get; }

        /// <summary>
        /// Number of messages currently delayed before entering the queue
        /// </summary>
        public Gauge DelayedMessageCount { get; }

        /// <summary>
        /// Number of messages currently in the queue (not including active messages)
        /// </summary>
        public Gauge MessageCount { get; }
    }
}
