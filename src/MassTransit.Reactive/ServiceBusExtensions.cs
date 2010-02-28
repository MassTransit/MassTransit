using System;
using MassTransit;

namespace MassTransit.Reactive
{
    public static class ServiceBusExtensions
    {
        public static IObservable<T> AsObservable<T>(this IServiceBus @this) where T : class
        {
            return new ServiceBusObserver<T>(@this);
        }

        public static IObservable<T> AsObservable<T>(this IServiceBus @this, Predicate<T> condition) where T : class
        {
            return new ServiceBusObserver<T>(@this, condition);
        }
    }
}
