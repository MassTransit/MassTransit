// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Internal;
	using Magnum;
	using Magnum.Reflection;
	using Magnum.Threading;
	using Serialization;
	using Transports;

    public class EndpointResolverConfigurator :
		IEndpointResolverConfigurator,
		IDisposable
	{
		private Type _defaultSerializer = typeof (XmlMessageSerializer);
		private volatile bool _disposed;
		private ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>> _endpointConfigurators;
		private IObjectBuilder _objectBuilder;
		private ReaderWriterLockedObject<HashSet<IEndpointFactory>> _transportTypes;

        //CHANGED to internal to help the move to the next configuration model
		internal EndpointResolverConfigurator()
		{
			_transportTypes = new ReaderWriterLockedObject<HashSet<IEndpointFactory>>(new HashSet<IEndpointFactory>());
			_endpointConfigurators = new ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>>();
		}

		public void SetDefaultSerializer<TSerializer>()
			where TSerializer : IMessageSerializer
		{
			_defaultSerializer = typeof (TSerializer);
		}

		public void SetDefaultSerializer(Type serializerType)
		{
			_defaultSerializer = serializerType;
		}

		public void RegisterTransport<TTransportFactory>()
			where TTransportFactory : IEndpointFactory
		{
			RegisterTransport(typeof (TTransportFactory));
		}

		public void RegisterTransport(Type transportType)
		{
		    var f = (IEndpointFactory)FastActivator.Create(transportType);
			_transportTypes.WriteLock(x =>
				{
					if (x.Contains(f))
						return;

					x.Add(f);
				});
		}

		public void ConfigureEndpoint(string uriString, Action<IEndpointConfigurator> action)
		{
			try
			{
				Uri uri = new Uri(uriString.ToLowerInvariant());

				ConfigureEndpoint(uri, action);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The endpoint Uri is invalid: " + uriString, ex);
			}
		}

		public void ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> action)
		{
			try
			{
				_endpointConfigurators.Store(uri, action);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException("The endpoint could not be configured: " + uri, ex);
			}
		}

		public void SetObjectBuilder(IObjectBuilder objectBuilder)
		{
			Guard.AgainstNull(objectBuilder, "objectBuilder must not be null");

			_objectBuilder = objectBuilder;
		}

		~EndpointResolverConfigurator()
		{
			Dispose(false);
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_endpointConfigurators.Dispose();
				_endpointConfigurators = null;

				_transportTypes.Dispose();
				_transportTypes = null;
			}
			_disposed = true;
		}

        //CHANGED TO SUPPORT THE MOVE TO THE NEW MODEL
		internal IEndpointResolver Create()
		{
			IEndpointResolver endpointResolver = new EndpointResolver(_defaultSerializer, _transportTypes.ReadLock(x => x), _endpointConfigurators);

			return endpointResolver;
		}

		public static IEndpointResolver New(Action<IEndpointResolverConfigurator> action)
		{
			using (var configurator = new EndpointResolverConfigurator())
			{
				action(configurator);

				return configurator.Create();
			}
		}
	}
}