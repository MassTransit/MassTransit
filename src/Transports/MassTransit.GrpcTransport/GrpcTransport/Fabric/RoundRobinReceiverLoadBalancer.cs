namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;
    using System.Threading;


    public class RoundRobinReceiverLoadBalancer :
        IReceiverLoadBalancer
    {
        Receiver _current;

        public RoundRobinReceiverLoadBalancer(IMessageReceiver[] receivers)
        {
            _current = BuildList(receivers.Copy().Shuffle());
        }

        public IMessageReceiver SelectReceiver(GrpcTransportMessage message)
        {
            Receiver selected;
            do
            {
                selected = _current;
            }
            while (Interlocked.CompareExchange(ref _current, selected.Next, selected) != selected);

            return selected.Current;
        }

        static Receiver BuildList(IReadOnlyList<IMessageReceiver> receivers)
        {
            var first = new Receiver(receivers[0]);
            var last = first;
            for (var i = 1; i < receivers.Count; i++)
            {
                var consumer = new Receiver(receivers[i]);
                last.Next = consumer;
                last = consumer;
            }

            last.Next = first;

            return last;
        }


        class Receiver
        {
            public Receiver(IMessageReceiver current)
            {
                Current = current;
            }

            public IMessageReceiver Current { get; }
            public Receiver Next { get; set; }
        }
    }
}
