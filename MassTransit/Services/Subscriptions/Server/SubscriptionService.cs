// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.Subscriptions.Server
{
	using System;
	using System.Linq;
	using Exceptions;
	using log4net;
	using Messages;
	using Saga;
	using Subscriptions.Messages;

	public class SubscriptionService :
		Consumes<SubscriptionClientAdded>.All,
		Consumes<SubscriptionClientRemoved>.All,
		Consumes<SubscriptionAdded>.All,
		Consumes<SubscriptionRemoved>.All,
		IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
		private readonly IEndpointFactory _endpointFactory;
		private readonly ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagas;
		private readonly ISagaRepository<SubscriptionSaga> _subscriptionSagas;
		private IServiceBus _bus;
		private ISubscriptionRepository _repository;
		private UnsubscribeAction _unsubscribeToken = () => false;

		public SubscriptionService(ISubscriptionRepository subscriptionRepository,
		                           IEndpointFactory endpointFactory,
		                           ISagaRepository<SubscriptionSaga> subscriptionSagas,
		                           ISagaRepository<SubscriptionClientSaga> subscriptionClientSagas)
		{
			_repository = subscriptionRepository;
			_endpointFactory = endpointFactory;
			_subscriptionSagas = subscriptionSagas;
			_subscriptionClientSagas = subscriptionClientSagas;
		}

		public void Consume(SubscriptionAdded message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Adding Subscription: {0}", message.Subscription);

			var add = new AddSubscription(message.Subscription);

			SendToClients(add);
		}

		public void Consume(SubscriptionClientAdded message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Sending cache update to {0}", message.EndpointUri);

			SendCacheUpdateToClient(message.EndpointUri);
		}

		public void Consume(SubscriptionClientRemoved message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Removing client: {0}", message.EndpointUri);
		}

		public void Consume(SubscriptionRemoved message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Removing Subscription: {0}", message.Subscription);

			var remove = new RemoveSubscription(message.Subscription);

			SendToClients(remove);
		}

		public void Dispose()
		{
			try
			{
				_bus.Dispose();
				_bus = null;

				_repository.Dispose();
				_repository = null;
			}
			catch (Exception ex)
			{
				string message = "Error in shutting down the SubscriptionService: " + ex.Message;
				ShutDownException exp = new ShutDownException(message, ex);
				_log.Error(message, exp);
				throw exp;
			}
		}

		public void Start(IServiceBus bus)
		{
			_log.Info("Subscription Service Starting");
		    _bus = bus;
			_unsubscribeToken += _bus.Subscribe(this);

			_unsubscribeToken += _bus.Subscribe<SubscriptionClientSaga>();
			_unsubscribeToken += _bus.Subscribe<SubscriptionSaga>();

			// TODO may need to load/prime the subscription repository at this point?

			_log.Info("Subscription Service Started");
		}

		public void Stop()
		{
			_log.Info("Subscription Service Stopping");

			_unsubscribeToken();

			_log.Info("Subscription Service Stopped");
		}

		private void SendToClients<T>(T message)
			where T : class
		{
			var clients = _subscriptionClientSagas
				.Where(x => x.CurrentState == SubscriptionClientSaga.Active);

			foreach (var client in clients)
			{
				IEndpoint endpoint = _endpointFactory.GetEndpoint(client.EndpointUri);

				endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Uri));
			}
		}

		private void SendCacheUpdateToClient(Uri uri)
		{
			var subscriptions = _subscriptionSagas
				.Where(x => x.CurrentState == SubscriptionSaga.Active)
				.Select(x => x.SubscriptionInfo);

			var message = new CacheUpdateResponse(subscriptions);

			IEndpoint endpoint = _endpointFactory.GetEndpoint(uri);

			endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Uri));
		}
	}
}