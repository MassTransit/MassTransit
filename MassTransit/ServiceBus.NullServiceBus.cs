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

            public Func<bool> Subscribe<T>(Action<T> callback) where T : class
            {
            	return () => false;
            }

            public Func<bool> Subscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
            {
				return () => false;
            }

            public Func<bool> Subscribe<T>(T consumer) where T : class
            {
				return () => false;
			}

            public Func<bool> Subscribe<TComponent>() where TComponent : class
            {
				return () => false;
			}

            public Func<bool> Subscribe(Type consumerType)
            {
				return () => false;
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