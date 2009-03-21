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
namespace MassTransit.Services.LoadBalancer.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Internal;

	public class LoadBalancerServiceConfigurator :
		ILoadBalancerServiceConfigurator
	{
		private readonly Dictionary<Type, ILoadBalancerStrategy> _types = new Dictionary<Type, ILoadBalancerStrategy>();
		public Type ServiceType
		{
			get { return typeof (ILoadBalancerService); }
		}

		public IBusService Create(IServiceBus bus, IObjectBuilder builder)
		{
			var arguments = new Hashtable
				{
					{ "bus", bus },
				};

			var service = builder.GetInstance<ILoadBalancerService>(arguments);

			service.AddTypes(_types);

			return service;
		}

		public void LoadBalance<T>()
		{
		}
	}

	public interface ILoadBalancerStrategy
	{
	}
}