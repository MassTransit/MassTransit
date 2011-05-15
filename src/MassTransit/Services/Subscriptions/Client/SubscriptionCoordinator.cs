// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using log4net;
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Pipeline;
	using Util;

	public class SubscriptionCoordinator :
		ISubscriptionService,
		IBusService,
		Consumes<AddSubscriptionClient>.All,
		Consumes<RemoveSubscriptionClient>.All,
		Consumes<SubscriptionRefresh>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All
	{
		const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		static readonly ILog _log = LogManager.GetLogger(typeof(SubscriptionCoordinator));
		static readonly ClientSubscriptionInfoMapper _mapper = new ClientSubscriptionInfoMapper();
		readonly IServiceBus _bus;
		readonly HashSet<string> _ignoredSubscriptions;
		readonly EndpointUriList _localEndpoints;
		readonly string _network;
		readonly IEndpoint _outboundEndpoint;
		readonly SequenceNumberGenerator _sequence;
		readonly IList<IBusService> _services;
		readonly IdempotentHashtable<Guid, ClientSubscriptionInformation> _subscriptions;
		readonly TypeConverter _typeConverter;
		readonly Uri _uri;

		Guid _clientId;
		RegistrationList<IEndpointSubscriptionEvent> _clients;
		volatile bool _disposed;

		UnsubscribeAction _unsubscribeAction;
		bool _multicast;

		public SubscriptionCoordinator(IServiceBus bus, IEndpoint outboundEndpoint, string network, bool multicast)
		{
			_bus = bus;
			_uri = _bus.Endpoint.Address.Uri;
			_network = network;
			_multicast = multicast;
			_outboundEndpoint = outboundEndpoint;

			_clients = new RegistrationList<IEndpointSubscriptionEvent>();
			_ignoredSubscriptions = new HashSet<string>
				{
					typeof (AddSubscription).ToMessageName(),
					typeof (RemoveSubscription).ToMessageName(),
					typeof (AddSubscriptionClient).ToMessageName(),
					typeof (SubscriptionRefresh).ToMessageName(),
					typeof (RemoveSubscriptionClient).ToMessageName(),
				};
			_localEndpoints = new EndpointUriList();
			_services = new List<IBusService>();
			_sequence = new SequenceNumberGenerator();
			_subscriptions = new IdempotentHashtable<Guid, ClientSubscriptionInformation>();
			_typeConverter = TypeDescriptor.GetConverter(typeof (string));
		}

		public void Consume(AddSubscription message)
		{
			if (ShouldIgnoreMessage(message))
				return;

			Add(message.Subscription);
		}

		public void Consume(AddSubscriptionClient message)
		{
			if (ShouldIgnoreMessage(message))
				return;

			if (_multicast)
			{
				IEnumerable<SubscriptionInformation> subscriptions = _subscriptions
					.Select(
						x => new SubscriptionInformation(x.ClientId, x.SequenceNumber, x.MessageName, x.CorrelationId, x.EndpointUri)
							{
								SubscriptionId = x.SubscriptionId
							});

				Send(new SubscriptionRefresh(subscriptions));
			}
		}

		public void Consume(RemoveSubscription message)
		{
			if (ShouldIgnoreMessage(message))
				return;

			Remove(message.Subscription);
		}

		public void Consume(RemoveSubscriptionClient message)
		{
			if (ShouldIgnoreMessage(message))
				return;
		}

		public void Consume(SubscriptionRefresh message)
		{
			if (ShouldIgnoreMessage(message))
				return;

			foreach (SubscriptionInformation subscription in message.Subscriptions)
			{
				Add(subscription);
			}

			if (OnRefresh != null)
				OnRefresh();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting subscription coordinator on network '{0}'", _network);

			ConnectBus(bus);

			IServiceBus controlBus = bus.ControlBus;
			if (controlBus != bus)
			{
				ConnectBus(controlBus);
			}

			_localEndpoints.Add(bus.Endpoint);
			_localEndpoints.Add(bus.ControlBus.Endpoint);

			_clientId = CombGuid.Generate();

			_unsubscribeAction = _bus.SubscribeInstance(this);

			SendAddSubscriptionClient(bus);
		}

		public void Stop()
		{
			var message = new RemoveSubscriptionClient(_clientId, _bus.ControlBus.Endpoint.Address.Uri, _bus.Endpoint.Address.Uri);
			Send(message);

			_unsubscribeAction();
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
			var info = new SubscriptionInformation(_clientId, _sequence.Next(), typeof (TMessage), correlationId.ToString(),
				endpointUri);

			return SendAddSubscription(info);
		}

		public UnregisterAction Register(IEndpointSubscriptionEvent consumer)
		{
			return _clients.Register(consumer);
		}

		public event Action OnRefresh;

		void ConnectBus(IServiceBus bus)
		{
			var publisher = new SubscriptionPublisher(this);
			publisher.Start(bus);
			_services.Add(publisher);

			var consumer = new SubscriptionConsumer(this);
			consumer.Start(bus);
			_services.Add(consumer);
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			_services.Each(service =>
				{
					service.Stop();
					service.Dispose();
				});
			_services.Clear();

			if (_clients != null)
			{
				_clients.Dispose();
				_clients = null;
			}
			_disposed = true;
		}

		bool ShouldIgnoreMessage<T>(T message)
		{
			if (_bus.Context().SourceAddress == _bus.Endpoint.Address.Uri)
			{
			   _log.Debug("Ignoring subscription because its source address equals the busses address"); 
				return true;
			}

			if (!string.Equals(_bus.Context().Network, _network))
			{
				_log.DebugFormat("Ignoring subscription because the network '{0}' != ours '{1}1", _bus.Context().Network, _network); 
				return true;
			}

			return false;
		}

		void Send<T>(T message)
			where T : class
		{
			_outboundEndpoint.Send(message, context =>
				{
					context.SetSourceAddress(_uri);
					context.SetNetwork(_network);
				});
		}

		void SendAddSubscriptionClient(IServiceBus bus)
		{
			var message = new AddSubscriptionClient(_clientId, bus.ControlBus.Endpoint.Address.Uri, bus.Endpoint.Address.Uri);
			Send(message);
		}

		void Remove(SubscriptionInformation subscription)
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

		void Add(SubscriptionInformation sub)
		{
			if (IgnoreIfLocalEndpoint(sub.EndpointUri))
				return;

			_log.Debug("SubscriptionClient Add: " + sub);

			var messageType = Type.GetType(sub.MessageName);
			if (messageType == null)
			{
				_log.InfoFormat("Unknown message type '{0}', unable to add subscription", sub.MessageName);
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

		UnsubscribeAction AddCorrelationSubscription(Type correlatedByType, string correlationValue, Type messageType,
		                                             Uri endpointUri)
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
					_log.Error("The correlationId in the subscription could not be converted to the CorrelatedBy type: " +
					           keyType.FullName);
					return null;
				}

				try
				{
					correlationId = _typeConverter.ConvertTo(correlationValue, keyType);
				}
				catch (Exception)
				{
					_log.Error("The correlationId in the subscription failed to be converted to the CorrelatedBy type: " +
					           keyType.FullName);
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

		UnsubscribeAction SendAddSubscription(SubscriptionInformation info)
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

			Send(add);
			return () =>
				{
					var remove = new RemoveSubscription(info, _sequence.Next());

					Send(remove);

					return true;
				};
		}

		UnsubscribeAction AddToClients<T>(Uri endpointUri)
			where T : class
		{
			UnsubscribeAction result = () => true;

			_clients.Each(x => { result += x.SubscribedTo<T>(endpointUri); });

			return result;
		}

		UnsubscribeAction AddToClientsWithCorrelation<T, K>(K key, Uri endpointUri)
			where T : class, CorrelatedBy<K>
		{
			UnsubscribeAction result = () => true;

			_clients.Each(x => { result += x.SubscribedTo<T, K>(key, endpointUri); });

			return result;
		}

		bool IgnoreIfLocalEndpoint(Uri endpointUri)
		{
		    var r = _localEndpoints.Contains(endpointUri);
            _log.Debug("Ignored a subscription because its endpoint was local.");

			return r;
		}

		~SubscriptionCoordinator()
		{
			Dispose(false);
		}


		static Type GetCorrelatedByType(string correlationId, Type messageType)
		{
			if (string.IsNullOrEmpty(correlationId))
				return null;

			return messageType.GetInterfaces()
				.Where(inteface => inteface.IsGenericType && inteface.GetGenericTypeDefinition() == typeof (CorrelatedBy<>))
				.FirstOrDefault();
		}
	}
}