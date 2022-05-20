namespace MassTransit.Transports.Fabric
{
    using System.Collections.Generic;
    using System.Threading;


    public class RoundRobinReceiverLoadBalancer<T> :
        IReceiverLoadBalancer<T>
        where T : class
    {
        Receiver _current;

        public RoundRobinReceiverLoadBalancer(IMessageReceiver<T>[] receivers)
        {
            _current = BuildList(receivers.Copy().Shuffle());
        }

        public IMessageReceiver<T> SelectReceiver(T message)
        {
            Receiver selected;
            do
            {
                selected = _current;
            }
            while (Interlocked.CompareExchange(ref _current, selected.Next, selected) != selected);

            return selected.Current;
        }

        static Receiver BuildList(IReadOnlyList<IMessageReceiver<T>> receivers)
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
            public Receiver(IMessageReceiver<T> current)
            {
                Current = current;
            }

            public IMessageReceiver<T> Current { get; }
            public Receiver Next { get; set; }
        }
    }
}
