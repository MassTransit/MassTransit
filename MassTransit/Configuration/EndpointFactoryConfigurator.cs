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
namespace MassTransit.Configuration
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Internal;
	using Magnum.Threading;
	using Serialization;
	using Util;

	public class EndpointFactoryConfigurator :
		IEndpointFactoryConfigurator,
		IDisposable
	{
		private Type _defaultSerializer = typeof (BinaryMessageSerializer);
		private volatile bool _disposed;
		private ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>> _endpointConfigurators;
		private IObjectBuilder _objectBuilder;
		private ReaderWriterLockedObject<HashSet<Type>> _transportTypes;

		private EndpointFactoryConfigurator()
		{
			_transportTypes = new ReaderWriterLockedObject<HashSet<Type>>(new HashSet<Type>());
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

		public void RegisterTransport<TTransport>()
			where TTransport : IEndpoint
		{
			RegisterTransport(typeof (TTransport));
		}

		public void RegisterTransport(Type transportType)
		{
			_transportTypes.WriteLock(x =>
				{
					if (x.Contains(transportType))
						return;

					x.Add(transportType);
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
			Guard.Against.Null(objectBuilder, "objectBuilder must not be null");

			_objectBuilder = objectBuilder;
		}

		~EndpointFactoryConfigurator()
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

		private IEndpointFactory Create()
		{
			IEndpointFactory endpointFactory = new EndpointFactory(_objectBuilder, _defaultSerializer, _transportTypes.ReadLock(x => x), _endpointConfigurators);

			return endpointFactory;
		}

		public static IEndpointFactory New(Action<IEndpointFactoryConfigurator> action)
		{
			using (var configurator = new EndpointFactoryConfigurator())
			{
				action(configurator);

				return configurator.Create();
			}
		}
	}
}