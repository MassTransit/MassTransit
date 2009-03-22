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
	using Configuration;
	using Exceptions;
	using Internal;
	using log4net;
	using Pipeline;

	public class LoadBalancerService :
		ILoadBalancerService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (LoadBalancerService));
		private static readonly Random _randomizer = new Random();
		private readonly Dictionary<Type, ILoadBalancerStrategy> _types = new Dictionary<Type, ILoadBalancerStrategy>();

		private readonly IObjectBuilder _builder;
		private IServiceBus _bus;
		private readonly IEndpointFactory _endpointFactory;

		public LoadBalancerService(IObjectBuilder builder, IEndpointFactory endpointFactory)
		{
			_builder = builder;
			_endpointFactory = endpointFactory;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Stop()
		{
		}

		public void Execute<T>(T message, Action<object>[] consumers)
			where T : class
		{
			if (consumers.Length == 0)
				throw new MessageException(typeof (T), "No subscriptions for " + typeof (T).FullName);

			Action<object> consumer = SelectConsumer(consumers);

			OutboundMessage.Set(x =>
				{
					x.SetSourceAddress(_bus.Endpoint.Uri);
					x.SetMessageType(typeof (T));
				});

			consumer(message);

			OutboundMessage.Headers.Reset();
		}

		public void AddTypes(IEnumerable<KeyValuePair<Type, ILoadBalancerStrategy>> types)
		{
			foreach (KeyValuePair<Type, ILoadBalancerStrategy> pair in types)
			{
				_types.Add(pair.Key, pair.Value);
			};
		}

		private Action<object> SelectConsumer(Action<object>[] consumers)
		{
			int index = _randomizer.Next(consumers.Length - 1);

			return consumers[index];
		}

		public void Dispose()
		{
		}
	}
}