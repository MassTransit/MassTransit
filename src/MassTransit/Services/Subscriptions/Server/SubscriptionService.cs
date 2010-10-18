// Copyright 2007-2010 The Apache Software Foundation.
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
	using Magnum.Fibers;
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
		private readonly Fiber _queue = new ThreadPoolFiber();

		public SubscriptionService(IServiceBus bus,
		                           ISubscriptionRepository subscriptionRepository,
		                           IEndpointFactory endpointFactory,
		                           ISagaRepository<SubscriptionSaga> subscriptionSagas,
		                           ISagaRepository<SubscriptionClientSaga> subscriptionClientSagas)
		{
			_bus = bus;
			_repository = subscriptionRepository;
			_endpointFactory = endpointFactory;
			_subscriptionSagas = subscriptionSagas;
			_subscriptionClientSagas = subscriptionClientSagas;
		}

		public void Consume(SubscriptionAdded message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Subscription Added: {0} [{1}]", message.Subscription, message.Subscription.CorrelationId);

			var add = new AddSubscription(message.Subscription);

			_queue.Add(() => SendToClients(add));
		}

		public void Consume(SubscriptionClientAdded message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Subscription Client Added: {0} [{1}]", message.ControlUri, message.ClientId);

			_queue.Add(() => SendCacheUpdateToClient(message.ControlUri));
		}

		public void Consume(SubscriptionClientRemoved message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Subscription Client Removed: {0} [{1}]", message.ControlUri, message.CorrelationId);
		}

		public void Consume(SubscriptionRemoved message)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Subscription Removed: {0} [{1}]", message.Subscription, message.Subscription.CorrelationId);

			var remove = new RemoveSubscription(message.Subscription);

			_queue.Add(()=>SendToClients(remove));
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

		public void Start()
		{
			_log.Info("Subscription Service Starting");
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
			_subscriptionClientSagas.Where(x => x.CurrentState == SubscriptionClientSaga.Active)
				.Each(client =>
					{
						IEndpoint endpoint = _endpointFactory.GetEndpoint(client.ControlUri);

						endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Uri));
					});
		}

		private void SendCacheUpdateToClient(Uri uri)
		{
			var subscriptions = _subscriptionSagas.Where(x => x.CurrentState == SubscriptionSaga.Active)
				.Select(x => x.SubscriptionInfo);

			var response = new SubscriptionRefresh(subscriptions);

			IEndpoint endpoint = _endpointFactory.GetEndpoint(uri);

			endpoint.Send(response, x => x.SetSourceAddress(_bus.Endpoint.Uri));
		}
	}
}