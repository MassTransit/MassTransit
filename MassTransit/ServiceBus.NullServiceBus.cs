namespace MassTransit
{
    using System;

    public partial class ServiceBus
    {

        public static IServiceBus Null
        {
            get; private set;
        }

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

            public UnsubscribeAction Subscribe<T>(Action<T> callback) where T : class
            {
            	return () => false;
            }

            public UnsubscribeAction Subscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
            {
				return () => false;
            }

            public UnsubscribeAction Subscribe<T>(T consumer) where T : class
            {
				return () => false;
			}

            public UnsubscribeAction Subscribe<TComponent>() where TComponent : class
            {
				return () => false;
			}

            public UnsubscribeAction Subscribe(Type consumerType)
            {
				return () => false;
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