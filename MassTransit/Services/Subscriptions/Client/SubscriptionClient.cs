namespace MassTransit.Services.Subscriptions.Client
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using Messages;
	using Util;

	public class SubscriptionClient :
		IBusService,
		ISubscriptionService,
		Consumes<CacheUpdateResponse>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionClient));
		private static readonly ClientSubscriptionInfoMapper _mapper = new ClientSubscriptionInfoMapper();
		private readonly IEndpointFactory _endpointFactory;
		private readonly EndpointList _localEndpoints = new EndpointList();
		private readonly IdempotentHashtable<Guid, ClientSubscriptionInformation> _subscriptions = new IdempotentHashtable<Guid, ClientSubscriptionInformation>();
		private IServiceBus _bus;
		private Guid _clientId;

		private readonly RegistrationList<IEndpointSubscriptionEvent> _clients = new RegistrationList<IEndpointSubscriptionEvent>();
		private SubscriptionConsumer _consumer;
		private SubscriptionPublisher _publisher;
		private IEndpoint _subscriptionServiceEndpoint;
		private UnsubscribeAction _unsubscribeAction;
		private readonly ManualResetEvent _ready = new ManualResetEvent(false);
		private static readonly HashSet<string> _ignoredSubscriptions;

		static SubscriptionClient()
		{
			_ignoredSubscriptions = new HashSet<string>
				{
					typeof (AddSubscription).ToMessageName(),
					typeof (RemoveSubscription).ToMessageName(),
					typeof (CacheUpdateRequest).ToMessageName(),
					typeof (CacheUpdateResponse).ToMessageName(),
					typeof (CancelSubscriptionUpdates).ToMessageName(),
				};
		}

		public TimeSpan StartTimeout { get; set; }

		public SubscriptionClient(IEndpointFactory endpointFactory)
		{
			_endpointFactory = endpointFactory;

			StartTimeout = 30.Seconds();
		}

		public Uri SubscriptionServiceUri { get; set; }

		public void Consume(AddSubscription message)
		{
			Add(message.Subscription);
		}

		public void Consume(CacheUpdateResponse message)
		{
			foreach (SubscriptionInformation subscription in message.Subscriptions)
			{
				Add(subscription);
			}

			_ready.Set();
		}

		public void Consume(RemoveSubscription message)
		{
			Remove(message.Subscription);
		}

		public void Dispose()
		{
			_consumer.Dispose();
			_publisher.Dispose();
		}

		public void Start(IServiceBus bus)
		{
			if(_log.IsDebugEnabled)
				_log.DebugFormat("Starting SubscriptionClient on {0}", bus.Endpoint.Uri);

			_bus = bus;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Getting endpoint for subscription service at {0}", SubscriptionServiceUri);

			_subscriptionServiceEndpoint = _endpointFactory.GetEndpoint(SubscriptionServiceUri);

			VerifyClientAndServiceNotOnSameEndpoint();

			_localEndpoints.Add(_bus.Endpoint);

			_clientId = CombGuid.Generate();
			_ready.Reset();

			ConnectToSubscriptionService(bus);

			WaitForSubscriptionServiceResponse();
		}

		private void WaitForSubscriptionServiceResponse()
		{
			if (_log.IsDebugEnabled)
				_log.Debug("Waiting for response from the subscription service");

			bool received = _ready.WaitOne(StartTimeout);
			if(!received)
			{
				throw new InvalidOperationException("Timeout waiting for subscription service to respond");
			}
		}

		private void ConnectToSubscriptionService(IServiceBus bus)
		{
			_publisher = new SubscriptionPublisher(this);
			_publisher.Start(bus);

			_consumer = new SubscriptionConsumer(this, _endpointFactory);
			_consumer.Start(bus);

			_unsubscribeAction = _bus.Subscribe(this);

			_subscriptionServiceEndpoint.Send(new CacheUpdateRequest(_clientId, _bus.Endpoint.Uri));
		}

		public void Stop()
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Stopping the SubscriptionClient on {0}", _bus.Endpoint.Uri);

			_subscriptionServiceEndpoint.Send(new CancelSubscriptionUpdates(_clientId, _bus.Endpoint.Uri));

			_unsubscribeAction();

			_consumer.Stop();
			_publisher.Stop();
		}

		public UnsubscribeAction SubscribedTo<T>(Uri endpointUri) where T : class
		{
			var info = new SubscriptionInformation(typeof (T), endpointUri);

			return SendAddSubscription(info);
		}

		public UnsubscribeAction SubscribedTo<T, K>(K correlationId, Uri endpointUri) where T : class, CorrelatedBy<K>
		{
			var info = new SubscriptionInformation(typeof (T), correlationId.ToString(), endpointUri);

			return SendAddSubscription(info);
		}

		public UnregisterAction Register(IEndpointSubscriptionEvent consumer)
		{
			return _clients.Register(consumer);
		}

		private void Remove(SubscriptionInformation subscription)
		{
			if (!_subscriptions.Contains(subscription.SubscriptionId))
				return;

			_log.Debug("SubscriptionClient Remove: " + subscription);

			lock (_subscriptions)
			{
				if (!_subscriptions.Contains(subscription.SubscriptionId)) return;

				var subscriptionInformation = _subscriptions[subscription.SubscriptionId];

				try
				{
					subscriptionInformation.Unsubscribe();
				}
				catch (Exception ex)
				{
					_log.Error("Error removing a subscription (object may have been disposed)", ex);
				}

				_subscriptions.Remove(subscriptionInformation.SubscriptionId);
			}
		}

		private void Add(SubscriptionInformation sub)
		{
			if (IgnoreIfLocalEndpoint(sub.EndpointUri))
				return;

			_log.Debug("SubscriptionClient Add: " + sub);

			Type messageType = Type.GetType(sub.MessageName);
			if (messageType == null)
			{
				_log.InfoFormat("Unknown message type {0}, unable to add subscription", sub.MessageName);
				return;
			}

			lock (_subscriptions)
			{
				if (_subscriptions.Contains(sub.SubscriptionId))
					return;

				var clientInfo = _mapper.Transform(sub);
				MethodInfo addToClientMethod = GetType().GetMethod("AddToClients", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

				MethodInfo genericMethod = addToClientMethod.MakeGenericMethod(messageType);

				var action = GenerateLambda(genericMethod);

				clientInfo.Unsubscribe = action(this, sub.EndpointUri);

				_subscriptions[sub.SubscriptionId] = clientInfo;
			}
		}

		private void VerifyClientAndServiceNotOnSameEndpoint()
		{
			if (!_bus.Endpoint.Uri.Equals(_subscriptionServiceEndpoint.Uri))
				return;

			var message = "The service bus and subscription service cannot use the same endpoint: " + _bus.Endpoint.Uri;
			throw new EndpointException(_bus.Endpoint, message);
		}

		private UnsubscribeAction SendAddSubscription(SubscriptionInformation info)
		{
			if (_ignoredSubscriptions.Contains(info.MessageName))
				return () => false;

			var add = new AddSubscription(info);

			ClientSubscriptionInformation clientInfo = _mapper.Transform(info);

			lock (_subscriptions)
			{
				_subscriptions.Add(clientInfo.SubscriptionId, clientInfo);
			}

			_subscriptionServiceEndpoint.Send(add);
			return () =>
				{
					var remove = new RemoveSubscription(info);
					_subscriptionServiceEndpoint.Send(remove);

					return true;
				};
		}

		private UnsubscribeAction AddToClients<T>(Uri endpointUri)
			where T : class
		{
			UnsubscribeAction result = () => false;

			_clients.Each(x => { result += x.SubscribedTo<T>(endpointUri); });

			return result;
		}

		private bool IgnoreIfLocalEndpoint(Uri endpointUri)
		{
			return _localEndpoints.Contains(endpointUri);
		}

		private static Func<SubscriptionClient, Uri, UnsubscribeAction> GenerateLambda(MethodInfo mi)
		{
			var instance = Expression.Parameter(typeof (SubscriptionClient), "client");
			var value = Expression.Parameter(typeof (Uri), "value");

			var del = Expression.Lambda<Func<SubscriptionClient, Uri, UnsubscribeAction>>(Expression.Call(instance, mi, value), new[]
				{
					instance, value
				}).Compile();

			return del;
		}
	}
}