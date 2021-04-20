namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Generic;


    public class RoundRobinConsumerLoadBalancer :
        IConsumerLoadBalancer
    {
        readonly Queue<IGrpcQueueConsumer> _consumers;

        public RoundRobinConsumerLoadBalancer(IEnumerable<IGrpcQueueConsumer> consumers)
        {
            _consumers = new Queue<IGrpcQueueConsumer>(consumers);

            if (_consumers.Count == 0)
                throw new ArgumentException("There must be at least one consumer", nameof(consumers));
        }

        public IGrpcQueueConsumer SelectConsumer(GrpcTransportMessage message)
        {
            lock (_consumers)
            {
                var consumer = _consumers.Dequeue();

                _consumers.Enqueue(consumer);

                return consumer;
            }
        }
    }
}
