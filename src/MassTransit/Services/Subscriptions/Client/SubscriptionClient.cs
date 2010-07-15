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
namespace MassTransit.Services.Subscriptions.Client
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Util;

	public class SubscriptionClient :
		IBusService,
		ISubscriptionService,
		Consumes<SubscriptionRefresh>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All
	{
		private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		private static readonly HashSet<string> _ignoredSubscriptions;
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionClient));
		private static readonly ClientSubscriptionInfoMapper _mapper = new ClientSubscriptionInfoMapper();
		private readonly RegistrationList<IEndpointSubscriptionEvent> _clients = new RegistrationList<IEndpointSubscriptionEvent>();
		private readonly IEndpointFactory _endpointFactory;
		private readonly EndpointList _localEndpoints = new EndpointList();
		private readonly ManualResetEvent _ready = new ManualResetEvent(false);
		private readonly SequenceNumberGenerator _sequence = new SequenceNumberGenerator();
		private readonly IdempotentHashtable<Guid, ClientSubscriptionInformation> _subscriptions = new IdempotentHashtable<Guid, ClientSubscriptionInformation>();
		private readonly TypeConverter _typeConverter = TypeDescriptor.GetConverter(typeof (string));
		private IServiceBus _bus;

		private Guid _clientId;
		private SubscriptionConsumer _consumer;
		private volatile bool _disposed;
		private SubscriptionPublisher _publisher;
		private IEndpoint _subscriptionServiceEndpoint;
		private UnsubscribeAction _unsubscribeAction;

		static SubscriptionClient()
		{
			_ignoredSubscriptions = new HashSet<string>
				{
					typeof (AddSubscription).ToMessageName(),
					typeof (RemoveSubscription).ToMessageName(),
					typeof (AddSubscriptionClient).ToMessageName(),
					typeof (SubscriptionRefresh).ToMessageName(),
					typeof (RemoveSubscriptionClient).ToMessageName(),
				};
		}

		public SubscriptionClient(IEndpointFactory endpointFactory)
		{
			_endpointFactory = endpointFactory;

			StartTimeout = 30.Seconds();
		}

		public TimeSpan StartTimeout { get; set; }

		public Uri SubscriptionServiceUri { get; set; }

		public void Consume(AddSubscription message)
		{
			Add(message.Subscription);
		}

		public void Consume(RemoveSubscription message)
		{
			Remove(message.Subscription);
		}

		public void Consume(SubscriptionRefresh message)
		{
			foreach (SubscriptionInformation subscription in message.Subscriptions)
			{
				Add(subscription);
			}

			_ready.Set();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting SubscriptionClient on {0}", bus.Endpoint.Uri);

			_bus = bus;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Getting endpoint for subscription service at {0}", SubscriptionServiceUri);

			_subscriptionServiceEndpoint = _endpointFactory.GetEndpoint(SubscriptionServiceUri);

			VerifyClientAndServiceNotOnSameEndpoint();

			_localEndpoints.Add(_bus.Endpoint);
			_localEndpoints.Add(_bus.ControlBus.Endpoint);

			_clientId = CombGuid.Generate();
			_ready.Reset();

			ConnectToSubscriptionService(bus);

			WaitForSubscriptionServiceResponse();
		}

		public void Stop()
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Stopping the SubscriptionClient on {0}", _bus.Endpoint.Uri);

			var message = new RemoveSubscriptionClient(_clientId, _bus.ControlBus.Endpoint.Uri, _bus.Endpoint.Uri);
			_subscriptionServiceEndpoint.Send(message);

			_unsubscribeAction();

			_consumer.Stop();
			_publisher.Stop();
		}

		public UnsubscribeAction SubscribedTo<TMessage>(Uri endpointUri)
			where TMessage : class
		{
			var info = new SubscriptionInformation(_clientId, _sequence.Next(), typeof (TMessage), endpointUri);

			return SendAddSubscription(info);
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId, Uri endpointUri)
			where TMessage : class, CorrelatedBy<TKey>
		{
			var info = new SubscriptionInformation(_clientId, _sequence.Next(), typeof (TMessage), correlationId.ToString(), endpointUri);

			return SendAddSubscription(info);
		}

		public UnregisterAction Register(IEndpointSubscriptionEvent consumer)
		{
			return _clients.Register(consumer);
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			_consumer.Dispose();
			_publisher.Dispose();

			_disposed = true;
		}

		private void WaitForSubscriptionServiceResponse()
		{
			if (_log.IsDebugEnabled)
				_log.Debug("Waiting for response from the subscription service");

			bool received = _ready.WaitOne(StartTimeout);
			if (!received)
			{
				throw new InvalidOperationException("Timeout waiting for subscription service to respond");
			}
		}

		private void ConnectToSubscriptionService(IServiceBus bus)
		{
			ConnectSubscriptionPublisherToBus(bus);

			if (bus != bus.ControlBus)
				ConnectSubscriptionPublisherToBus(bus.ControlBus);

			_unsubscribeAction = _bus.ControlBus.Subscribe(this);

			var message = new AddSubscriptionClient(_clientId, _bus.ControlBus.Endpoint.Uri, _bus.Endpoint.Uri);
			_subscriptionServiceEndpoint.Send(message);
		}

		private void ConnectSubscriptionPublisherToBus(IServiceBus bus)
		{
			_publisher = new SubscriptionPublisher(this);
			_publisher.Start(bus);

			_consumer = new SubscriptionConsumer(this, _endpointFactory);
			_consumer.Start(bus);
		}

		private void Remove(SubscriptionInformation subscription)
		{
			if (!_subscriptions.Contains(subscription.SubscriptionId))
				return;

			_log.Debug("SubscriptionClient Remove: " + subscription);

			lock (_subscriptions)
			{
				if (!_subscriptions.Contains(subscription.SubscriptionId)) return;

				ClientSubscriptionInformation subscriptionInformation = _subscriptions[subscription.SubscriptionId];

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

				ClientSubscriptionInformation clientInfo = _mapper.Transform(sub);

				UnsubscribeAction unsubscribe;

				Type correlatedByType = GetCorrelatedByType(sub.CorrelationId, messageType);
				if (correlatedByType != null)
				{
					unsubscribe = AddCorrelationSubscription(correlatedByType, sub.CorrelationId, messageType, sub.EndpointUri);
				}
				else
				{
					unsubscribe = (UnsubscribeAction) GetType()
					                                  	.GetMethod("AddToClients", _bindingFlags)
					                                  	.MakeGenericMethod(messageType).Invoke(this, new[] {sub.EndpointUri});
				}

				clientInfo.Unsubscribe = unsubscribe;

				_subscriptions[sub.SubscriptionId] = clientInfo;
			}
		}

		private UnsubscribeAction AddCorrelationSubscription(Type correlatedByType, string correlationValue, Type messageType, Uri endpointUri)
		{
			Type keyType = correlatedByType.GetGenericArguments().First();

			object correlationId;
			if (keyType == typeof (Guid))
			{
				try
				{
					correlationId = new Guid(correlationValue);
				}
				catch (FormatException)
				{
					_log.Error("The correlationId in the subscription could not be converted to a Guid: " + correlationValue);
					return null;
				}
			}
			else
			{
				if (!_typeConverter.CanConvertTo(keyType))
				{
					_log.Error("The correlationId in the subscription could not be converted to the CorrelatedBy type: " + keyType.FullName);
					return null;
				}

				try
				{
					correlationId = _typeConverter.ConvertTo(correlationValue, keyType);
				}
				catch (Exception)
				{
					_log.Error("The correlationId in the subscription failed to be converted to the CorrelatedBy type: " + keyType.FullName);
					return null;
				}
			}

			if (correlationId == null)
				return null;

			return (UnsubscribeAction) GetType()
			                           	.GetMethod("AddToClientsWithCorrelation", _bindingFlags)
			                           	.MakeGenericMethod(messageType, keyType)
			                           	.Invoke(this, new[] {correlationId, endpointUri});
		}

		private void VerifyClientAndServiceNotOnSameEndpoint()
		{
			if (!_bus.ControlBus.Endpoint.Uri.Equals(_subscriptionServiceEndpoint.Uri))
				return;

			string message = "The service bus and subscription service cannot use the same endpoint: " + _bus.ControlBus.Endpoint.Uri;
			throw new EndpointException(_bus.ControlBus.Endpoint.Uri, message);
		}

		private UnsubscribeAction SendAddSubscription(SubscriptionInformation info)
		{
			if (_ignoredSubscriptions.Contains(info.MessageName))
				return () => true;

			var add = new AddSubscription(info);

			ClientSubscriptionInformation clientInfo = _mapper.Transform(info);

			if (clientInfo.Unsubscribe == null)
			{
				clientInfo.Unsubscribe = () => true;
			}

			lock (_subscriptions)
			{
				_subscriptions.Add(clientInfo.SubscriptionId, clientInfo);
			}

			_subscriptionServiceEndpoint.Send(add);
			return () =>
				{
					var remove = new RemoveSubscription(info, _sequence.Next());

					_subscriptionServiceEndpoint.Send(remove);

					return true;
				};
		}

		private UnsubscribeAction AddToClients<T>(Uri endpointUri)
			where T : class
		{
			UnsubscribeAction result = () => true;

			_clients.Each(x => { result += x.SubscribedTo<T>(endpointUri); });

			return result;
		}

		private UnsubscribeAction AddToClientsWithCorrelation<T, K>(K key, Uri endpointUri)
			where T : class, CorrelatedBy<K>
		{
			UnsubscribeAction result = () => true;

			_clients.Each(x => { result += x.SubscribedTo<T, K>(key, endpointUri); });

			return result;
		}

		private bool IgnoreIfLocalEndpoint(Uri endpointUri)
		{
			return _localEndpoints.Contains(endpointUri);
		}

		~SubscriptionClient()
		{
			Dispose(false);
		}

		private static Type GetCorrelatedByType(string correlationId, Type messageType)
		{
			if (string.IsNullOrEmpty(correlationId))
				return null;

			return messageType.GetInterfaces()
				.Where(inteface => inteface.IsGenericType && inteface.GetGenericTypeDefinition() == typeof (CorrelatedBy<>))
				.FirstOrDefault();
		}
	}
}