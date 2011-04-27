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
namespace MassTransit.Infrastructure.Configurators
{
	using System.Collections.Generic;
	using Builders;
	using BusConfigurators;
	using Exceptions;
	using Internal;
	using log4net;
	using MassTransit.Configurators;
	using NHibernate;
	using Saga;

	public class NHibernateSagaRepositoryConfiguratorImpl :
		BusBuilderConfigurator
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (NHibernateSagaRepositoryConfiguratorImpl));
		readonly ISessionFactory _sessionFactory;

		public NHibernateSagaRepositoryConfiguratorImpl(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.Match<ServiceBusBuilder>(x =>
				{
					IObjectBuilder objectBuilder = builder.Settings.ObjectBuilder;
					if (objectBuilder == null)
						throw new ConfigurationException("The object builder is null and cannot be modified.");

					var fastActivatorObjectBuilder = objectBuilder as FastActivatorObjectBuilder;
					if (fastActivatorObjectBuilder == null)
						throw new ConfigurationException(
							"Only the default object builder can be modified to use the NHibernateSagaRepository");

					if (_log.IsDebugEnabled)
						_log.DebugFormat("Using the NHibernateSagaRepository for {0}", builder.Settings.InputAddress);

					fastActivatorObjectBuilder.SetSagaRepositoryFactory(new NHibernateSagaRepositoryFactory(_sessionFactory));
				});

			return builder;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_sessionFactory == null)
				yield return this.Failure("SessionFactory", "The session factory must not be null.");
		}
	}
}