namespace MassTransit
{
    using System;

    public partial class ServiceBus
    {
        private class NullServiceBus : IServiceBus
        {
            public void Dispose()
            {
            }

            public IEndpoint Endpoint
            {
                get { return null; }
            }

            public IEndpoint PoisonEndpoint
            {
                get { return null; }
            }

            public void Subscribe<T>(Action<T> callback) where T : class
            {
            }

            public void Subscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
            {
            }

            public void Subscribe<T>(T consumer) where T : class
            {
            }

            public void Subscribe<TComponent>() where TComponent : class
            {
            }

            public void Subscribe(Type consumerType)
            {
            }

            public void Unsubscribe<T>(Action<T> callback) where T : class
            {
            }

            public void Unsubscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
            {
            }

            public void Unsubscribe<T>(T consumer) where T : class
            {
            }

            public void Unsubscribe(Type consumerType)
            {
            }

            public void Unsubscribe<TComponent>() where TComponent : class
            {
            }

            public void Publish<T>(T message) where T : class
            {
            }

            public RequestBuilder Request()
            {
                throw new NotImplementedException();
            }
        }
    }
}