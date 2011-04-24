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
	using Configurators;
	using MassTransit.Builders;
	using MassTransit.Exceptions;

	public class DefaultTransactionTimeoutEndpointFactoryConfigurator :
		EndpointFactoryBuilderConfigurator
	{
		readonly TimeSpan _transactionTimeout;

		public DefaultTransactionTimeoutEndpointFactoryConfigurator(TimeSpan transactionTimeout)
		{
			_transactionTimeout = transactionTimeout;
		}

		public void Validate()
		{
			if (_transactionTimeout <= TimeSpan.Zero)
				throw new ConfigurationException("The transaction timeout must be greater than zero");
		}

		public EndpointFactoryBuilder Configure(EndpointFactoryBuilder builder)
		{
			builder.SetDefaultTransactionTimeout(_transactionTimeout);

			return builder;
		}
	}
}