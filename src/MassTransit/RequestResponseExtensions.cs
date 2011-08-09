namespace MassTransit
{
	using System;
	using RequestResponse;
	using RequestResponse.Configurators;

	public static class RequestResponseExtensions
	{
		public static bool PublishRequest<TRequest>(this IServiceBus bus,
		                                            TRequest message,
		                                            Action<RequestConfigurator<TRequest, Guid>> configureCallback)
			where TRequest : class, CorrelatedBy<Guid>
		{
			return PublishRequest<TRequest, Guid>(bus, message, configureCallback);
		}

		public static bool PublishRequest<TRequest, TKey>(this IServiceBus bus,
		                                                  TRequest message,
		                                                  Action<RequestConfigurator<TRequest, TKey>> configureCallback)
			where TRequest : class, CorrelatedBy<TKey>
		{
			IRequest<TRequest, TKey> request = RequestConfiguratorImpl<TRequest, TKey>.Create(bus, message, configureCallback);

			bus.Publish(message, context => context.SendResponseTo(bus.Endpoint.Address.Uri));

			return request.Wait();
		}

		public static IAsyncResult BeginPublishRequest<TRequest>(this IServiceBus bus,
		                                                         TRequest message,
		                                                         AsyncCallback callback,
		                                                         object state,
		                                                         Action<RequestConfigurator<TRequest, Guid>> configureCallback)
			where TRequest : class, CorrelatedBy<Guid>
		{
			return BeginPublishRequest<TRequest, Guid>(bus, message, callback, state, configureCallback);
		}

		public static IAsyncResult BeginPublishRequest<TRequest, TKey>(this IServiceBus bus,
		                                                               TRequest message,
		                                                               AsyncCallback callback,
		                                                               object state,
		                                                               Action<RequestConfigurator<TRequest, TKey>>
		                                                               	configureCallback)
			where TRequest : class, CorrelatedBy<TKey>
		{
			IRequest<TRequest, TKey> request = RequestConfiguratorImpl<TRequest, TKey>.Create(bus, message, configureCallback);

			bus.Publish(message, context => context.SendResponseTo(bus.Endpoint.Address.Uri));

			return request.BeginAsync(callback, state);
		}


		public static bool EndRequest(this IServiceBus bus, IAsyncResult asyncResult)
		{
			var request = asyncResult as IRequest;
			if (request == null)
				throw new ArgumentException("The argument is not an IRequest");

			return request.Wait();
		}

		public static bool SendRequest<TRequest>(this IEndpoint endpoint,
		                                         TRequest message,
		                                         IServiceBus bus,
		                                         Action<RequestConfigurator<TRequest, Guid>> configureCallback)
			where TRequest : class, CorrelatedBy<Guid>
		{
			return SendRequest<TRequest, Guid>(endpoint, message, bus, configureCallback);
		}

		public static bool SendRequest<TRequest, TKey>(this IEndpoint endpoint,
		                                               TRequest message,
		                                               IServiceBus bus,
		                                               Action<RequestConfigurator<TRequest, TKey>> configureCallback)
			where TRequest : class, CorrelatedBy<TKey>
		{
			IRequest<TRequest, TKey> request = RequestConfiguratorImpl<TRequest, TKey>.Create(bus, message, configureCallback);

			endpoint.Send(message, context => context.SendResponseTo(bus.Endpoint.Address.Uri));

			return request.Wait();
		}

		public static IAsyncResult BeginSendRequest<TRequest>(this IEndpoint endpoint,
		                                                      TRequest message,
		                                                      IServiceBus bus,
		                                                      AsyncCallback callback,
		                                                      object state,
		                                                      Action<RequestConfigurator<TRequest, Guid>> configureCallback)
			where TRequest : class, CorrelatedBy<Guid>
		{
			return BeginSendRequest(endpoint, bus, message, callback, state, configureCallback);
		}

		public static IAsyncResult BeginSendRequest<TRequest, TKey>(this IEndpoint endpoint,
		                                                            IServiceBus bus,
		                                                            TRequest message,
		                                                            AsyncCallback callback,
		                                                            object state,
		                                                            Action<RequestConfigurator<TRequest, TKey>>
		                                                            	configureCallback)
			where TRequest : class, CorrelatedBy<TKey>
		{
			IRequest<TRequest, TKey> request = RequestConfiguratorImpl<TRequest, TKey>.Create(bus, message, configureCallback);

			endpoint.Send(message, context => context.SendResponseTo(bus.Endpoint.Address.Uri));

			return request.BeginAsync(callback, state);
		}


		public static bool EndRequest(this IEndpoint endpoint, IAsyncResult asyncResult)
		{
			var request = asyncResult as IRequest;
			if (request == null)
				throw new ArgumentException("The argument is not an IRequest");

			return request.Wait();
		}
	}
}