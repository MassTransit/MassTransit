namespace MassTransit.Transports.Metrics
{
    using System;
    using System.Threading;


    public class DeliveryTracker :
        IDeliveryTracker
    {
        readonly ZeroActiveDeliveryHandler _handler;
        int _activeDeliveryCount;
        long _deliveryCount;
        int _maxConcurrentDeliveryCount;

        public DeliveryTracker(ZeroActiveDeliveryHandler handler)
        {
            _handler = handler;
        }

        public int ActiveDeliveryCount => _activeDeliveryCount;
        public long DeliveryCount => _deliveryCount;
        public int MaxConcurrentDeliveryCount => _maxConcurrentDeliveryCount;

        public IDelivery BeginDelivery()
        {
            var current = Interlocked.Increment(ref _activeDeliveryCount);
            while (current > _maxConcurrentDeliveryCount)
                Interlocked.CompareExchange(ref _maxConcurrentDeliveryCount, current, _maxConcurrentDeliveryCount);

            return new Delivery(Interlocked.Increment(ref _deliveryCount), DeliveryComplete);
        }

        public void DeliveryComplete(long id)
        {
            var pendingCount = Interlocked.Decrement(ref _activeDeliveryCount);
            if (pendingCount == 0)
            {
                _handler();
            }
        }


        readonly struct Delivery :
            IDelivery
        {
            readonly long _id;
            readonly Action<long> _complete;

            long IDelivery.Id => _id;

            public Delivery(long id, Action<long> complete)
            {
                _id = id;
                _complete = complete;
            }

            public void Dispose()
            {
                _complete(_id);
            }
        }
    }
}
