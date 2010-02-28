using System;
using MassTransit;

namespace MassTransit.Reactive
{
    internal class ServiceBusObserver<T> : IObservable<T> where T : class
    {
        private readonly IServiceBus bus;
        private readonly Predicate<T> condition;

        public ServiceBusObserver(IServiceBus bus)
            : this(bus, null)
        {
        }

        public ServiceBusObserver(IServiceBus bus, Predicate<T> condition)
        {
            this.bus = bus;
            this.condition = condition;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return new ServiceBusSubscription(bus, observer, condition);
        }

        private class ServiceBusSubscription : IDisposable
        {
            private UnsubscribeAction unsubscribeAction;

            public ServiceBusSubscription(IServiceBus bus, IObserver<T> observer, Predicate<T> condition)
            {
                unsubscribeAction = condition == null ?
                    bus.Subscribe<T>(observer.OnNext) :
                    bus.Subscribe<T>(observer.OnNext, condition);

                // TODO: Hook for observer.OnError?
            }

            public void Dispose()
            {
                unsubscribeAction();
            }
        }
    }
}
