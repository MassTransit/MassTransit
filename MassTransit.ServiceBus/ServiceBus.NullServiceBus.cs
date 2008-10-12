namespace MassTransit.ServiceBus
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

            public void Subscribe<T>(Action<IMessageContext<T>> callback) where T : class
            {
            }

            public void Subscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class
            {
            }

            public void Subscribe<T>(T component) where T : class
            {
            }

            public void Unsubscribe<T>(Action<IMessageContext<T>> callback) where T : class
            {
            }

            public void Unsubscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class
            {
            }

            public void Unsubscribe<T>(T component) where T : class
            {
            }

            public void AddComponent<TComponent>() where TComponent : class
            {
            }

            public void RemoveComponent<TComponent>() where TComponent : class
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