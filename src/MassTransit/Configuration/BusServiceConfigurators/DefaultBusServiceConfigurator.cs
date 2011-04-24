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
namespace MassTransit.Configurators
{
	using System;
	using Builders;
	using BusConfigurators;
	using Exceptions;
	using Internal;

	public class DefaultBusServiceConfigurator<TService> :
		IBusServiceConfigurator,
		BusBuilderConfigurator
		where TService : IBusService
	{
		readonly Func<IServiceBus, TService> _serviceFactory;

		public DefaultBusServiceConfigurator(Func<TService> serviceFactory)
		{
			_serviceFactory = bus => serviceFactory();
		}

		public DefaultBusServiceConfigurator(Func<IServiceBus, TService> serviceFactory)
		{
			_serviceFactory = serviceFactory;
		}

		public Type ServiceType
		{
			get { return typeof (TService); }
		}

		public IBusService Create(IServiceBus bus)
		{
			return _serviceFactory(bus);
		}

		public void Validate()
		{
			if (_serviceFactory == null)
				throw new ConfigurationException("The bus service factory can not be null: " + typeof (TService).Name);
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.Match<ServiceBusBuilder>(x => x.AddBusServiceConfigurator(this));

			return builder;
		}
	}
}