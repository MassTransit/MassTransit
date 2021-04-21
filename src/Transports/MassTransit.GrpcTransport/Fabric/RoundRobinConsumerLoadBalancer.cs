namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;
    using System.Threading;


    public class RoundRobinConsumerLoadBalancer :
        IConsumerLoadBalancer
    {
        Consumer _current;

        public RoundRobinConsumerLoadBalancer(IGrpcQueueConsumer[] consumers)
        {
            _current = BuildList(consumers.Copy().Shuffle());
        }

        public IGrpcQueueConsumer SelectConsumer(GrpcTransportMessage message)
        {
            Consumer selected;
            do
            {
                selected = _current;
            }
            while (Interlocked.CompareExchange(ref _current, selected.Next, selected) != selected);

            return selected.Current;
        }

        static Consumer BuildList(IReadOnlyList<IGrpcQueueConsumer> consumers)
        {
            var first = new Consumer(consumers[0]);
            var last = first;
            for (var i = 1; i < consumers.Count; i++)
            {
                var consumer = new Consumer(consumers[i]);
                last.Next = consumer;
                last = consumer;
            }

            last.Next = first;

            return last;
        }


        class Consumer
        {
            public Consumer(IGrpcQueueConsumer current)
            {
                Current = current;
            }

            public IGrpcQueueConsumer Current { get; }
            public Consumer Next { get; set; }
        }
    }
}
