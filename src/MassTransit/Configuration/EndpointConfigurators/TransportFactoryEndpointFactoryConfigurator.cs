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
namespace MassTransit.EndpointConfigurators
{
	using System;
	using Builders;
	using Exceptions;
	using Transports;

	public class TransportFactoryEndpointFactoryConfigurator :
		EndpointFactoryBuilderConfigurator
	{
		readonly Func<ITransportFactory> _transportFactory;

		public TransportFactoryEndpointFactoryConfigurator(Func<ITransportFactory> transportFactory)
		{
			_transportFactory = transportFactory;
		}

		public void Validate()
		{
			if (_transportFactory == null)
				throw new ConfigurationException("A null transport factory was specified");
		}

		public EndpointFactoryBuilder Configure(EndpointFactoryBuilder builder)
		{
			ITransportFactory transportFactory = _transportFactory();
			if (transportFactory == null)
				throw new ConfigurationException("A transport factory was not created");

			builder.AddTransportFactory(transportFactory);

			return builder;
		}
	}
}