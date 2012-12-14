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
namespace MassTransit.Transports.RabbitMq.Configuration.Configurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Builders;
	using MassTransit.Configurators;

	public class ConnectionFactoryConfiguratorImpl :
		ConnectionFactoryConfigurator,
		RabbitMqTransportFactoryBuilderConfigurator
	{
		readonly IRabbitMqEndpointAddress _address;

		readonly List<ConnectionFactoryBuilderConfigurator> _configurators;

		public ConnectionFactoryConfiguratorImpl(IRabbitMqEndpointAddress address)
		{
			_address = address;
			_configurators = new List<ConnectionFactoryBuilderConfigurator>();
		}


		public IEnumerable<ValidationResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate());
		}

		public void UseSsl(Action<SslConnectionFactoryConfigurator> configureSsl)
		{
			var configurator = new SslConnectionFactoryConfiguratorImpl();

			configureSsl(configurator);

			_configurators.Add(configurator);
		}

		public void SetRequestedHeartbeat(ushort requestedHeartbeat)
		{
			_configurators.Add(new RequestedHeartbeatConnectionFactoryConfigurator(requestedHeartbeat));
		}

	    public void SetUsername(string username)
	    {
	        _configurators.Add(new UsernameConnectionFactoryConfigurator(username));
	    }

	    public void SetPassword(string password)
	    {
	        _configurators.Add(new PasswordConnectionFactoryConfigurator(password));
	    }

	    public RabbitMqTransportFactoryBuilder Configure(RabbitMqTransportFactoryBuilder builder)
		{
			ConnectionFactoryBuilder connectionFactoryBuilder = CreateBuilder();

			builder.AddConnectionFactoryBuilder(_address.Uri, connectionFactoryBuilder);

			return builder;
		}

		public ConnectionFactoryBuilder CreateBuilder()
		{
			var connectionFactoryBuilder = new ConnectionFactoryBuilderImpl(_address);

			_configurators.Aggregate((ConnectionFactoryBuilder) connectionFactoryBuilder,
				(seed, configurator) => configurator.Configure(seed));
			return connectionFactoryBuilder;
		}
	}
}