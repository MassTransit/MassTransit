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
namespace MassTransit.Services.LoadBalancer
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Internal;
	using log4net;
	using MassTransit.Subscriptions;

	public class LoadBalancerService :
		ILoadBalancerService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (LoadBalancerService));
		private static readonly Random _randomizer = new Random();

		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly IEndpointFactory _endpointFactory;
		private readonly ITypeInfoCache _typeInfoCache;

		public LoadBalancerService(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_bus = bus;
			_cache = cache;
			_builder = builder;

			_typeInfoCache = _builder.GetInstance<ITypeInfoCache>();
			_endpointFactory = _builder.GetInstance<IEndpointFactory>();
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		public void Execute<T>(T message)
			where T : class
		{
			IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo<T>();

			IList<Subscription> subs = info.GetConsumers(_cache, message);

			if (subs.Count == 0)
				throw new MessageException(typeof (T), "No subscriptions for " + typeof (T).FullName);

			IEndpoint endpoint = SelectNode(subs);

			OutboundMessage.Set(x =>
				{
					x.SetSourceAddress(_bus.Endpoint.Uri);
					x.SetDestinationAddress(endpoint.Uri);
					x.SetMessageType(typeof (T));
				});

			endpoint.Send(message, info.TimeToLive);

			OutboundMessage.Headers.Reset();
		}

		private IEndpoint SelectNode(IList<Subscription> subs)
		{
			int index = _randomizer.Next(subs.Count - 1);

			var subscription = subs[index];

			IEndpoint endpoint = _endpointFactory.GetEndpoint(subscription.EndpointUri);

			return endpoint;
		}

		public void Dispose()
		{
		}
	}
}