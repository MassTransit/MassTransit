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
namespace MassTransit.Transports
{
	using System;
	using System.Collections.Generic;
	using Builders;
	using EndpointConfigurators;
	using Exceptions;
	using Magnum.Extensions;

	public class EndpointFactory :
		IEndpointFactory
	{
		readonly IEndpointFactoryDefaultSettings _defaults;
		readonly IDictionary<Uri, EndpointBuilder> _endpointBuilders;
		readonly IDictionary<string, ITransportFactory> _transportFactories;
		bool _disposed;

		public EndpointFactory(IDictionary<string, ITransportFactory> transportFactories,
		                       IDictionary<Uri, EndpointBuilder> endpointBuilders, IEndpointFactoryDefaultSettings defaults)
		{
			_transportFactories = transportFactories;
			_defaults = defaults;
			_endpointBuilders = endpointBuilders;
		}

		public IEndpoint CreateEndpoint(Uri uri)
		{
			string scheme = uri.Scheme.ToLowerInvariant();

			ITransportFactory transportFactory;
			if (_transportFactories.TryGetValue(scheme, out transportFactory))
			{
				try
				{
					EndpointBuilder builder = _endpointBuilders.Retrieve(uri, () =>
						{
							var configurator = new EndpointConfiguratorImpl(uri, _defaults);

							return configurator.CreateBuilder();
						});

					return builder.CreateEndpoint(transportFactory);
				}
				catch (Exception ex)
				{
					throw new EndpointException(uri, "Failed to create endpoint", ex);
				}
			}

			throw new ConfigurationException("The {0} scheme was not handled by any registered transport.".FormatWith(uri.Scheme));
		}

		public void AddTransportFactory(ITransportFactory factory)
		{
			string scheme = factory.Scheme.ToLowerInvariant();

			_transportFactories[scheme] = factory;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_transportFactories.Values.Each(x => { x.Dispose(); });
			}

			_disposed = true;
		}

		~EndpointFactory()
		{
			Dispose(false);
		}
	}
}