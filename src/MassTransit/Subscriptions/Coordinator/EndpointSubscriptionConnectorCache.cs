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
namespace MassTransit.Subscriptions.Coordinator
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using Magnum.Reflection;
	using log4net;

	public class EndpointSubscriptionConnectorCache
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (EndpointSubscriptionConnectorCache));

		readonly Dictionary<Type, EndpointSubscriptionConnector> _cache;
		readonly TypeConverter _typeConverter;
		IServiceBus _bus;

		public EndpointSubscriptionConnectorCache(IServiceBus bus)
		{
			_bus = bus;
			_typeConverter = TypeDescriptor.GetConverter(typeof (string));
			_cache = new Dictionary<Type, EndpointSubscriptionConnector>();
		}

		public UnsubscribeAction Connect(string messageName, Uri endpointUri, string correlationId)
		{
			Type messageType = Type.GetType(messageName);
			if (messageType == null)
			{
				_log.InfoFormat("Unknown message type '{0}', unable to add subscription", messageName);
				return () => true;
			}

			EndpointSubscriptionConnector connector;
			if (!_cache.TryGetValue(messageType, out connector))
			{
				connector = (EndpointSubscriptionConnector) FastActivator.Create(typeof (EndpointSubscriptionConnector<>),
					new[] {messageType}, new object[] {_bus});

				_cache.Add(messageType, connector);
			}

			return connector.Connect(endpointUri, correlationId);
		}

		Func<string, TKey> GetKeyConverter<TKey>()
		{
			Type keyType = typeof (TKey);

			object correlationId;
			if (keyType == typeof (Guid))
			{
				return x => (TKey) ((object) new Guid(x));
			}

			if (_typeConverter.CanConvertTo(keyType))
			{
				return x => (TKey) _typeConverter.ConvertTo(x, keyType);
			}

			throw new InvalidOperationException(
				"The correlationId in the subscription could not be converted to the CorrelatedBy type: " +
				keyType.FullName);
		}
	}
}