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
namespace MassTransit.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Configuration;
	using Exceptions;
	using Magnum;
	using Magnum.Threading;

    public class EndpointFactory :
		IEndpointFactory
	{
		private static readonly IEndpoint _null = new NullEndpoint();
		private readonly Type _defaultSerializer;
		private readonly IObjectBuilder _objectBuilder;
		private volatile bool _disposed;
		private ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>> _endpointConfigurators;
		private ReaderWriterLockedDictionary<Uri, IEndpoint> _endpoints;
		private ReaderWriterLockedDictionary<Type, Func<Uri, Action<IEndpointConfigurator>, IEndpoint>> _transportConfigurators;

		public EndpointFactory(IObjectBuilder objectBuilder,
		                       Type defaultSerializer,
		                       IEnumerable<Type> transportTypes,
		                       IEnumerable<KeyValuePair<Uri, Action<IEndpointConfigurator>>> endpointConfigurators)
		{
			_transportConfigurators = new ReaderWriterLockedDictionary<Type, Func<Uri, Action<IEndpointConfigurator>, IEndpoint>>();
			_endpointConfigurators = new ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>>(endpointConfigurators);
			_endpoints = new ReaderWriterLockedDictionary<Uri, IEndpoint>();

			_defaultSerializer = defaultSerializer;
			_objectBuilder = objectBuilder;

			ConnectTransportConfigurators(transportTypes);
		}

		public static IEndpoint Null
		{
			get { return _null; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEndpoint GetEndpoint(string uriString)
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			try
			{
				Uri uri = new Uri(uriString);

				return GetEndpoint(uri);
			}
			catch (UriFormatException ex)
			{
				throw new ArgumentException("The endpoint Uri is invalid: " + uriString, ex);
			}
		}

        public IEndpoint GetEndpoint(Uri uri)
        {
            Guard.AgainstNull(uri);
            if (_disposed) throw new ObjectDisposedException("The object has been disposed");

            try
            {
                Uri key = new Uri(uri.ToString().ToLowerInvariant());

                return _endpoints.Retrieve(key, () => BuildEndpoint(uri));
            }
            catch (EndpointException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConfigurationException("An error occurred retrieving the endpoint for " + uri, ex);
            }
        }

        protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_endpointConfigurators.Dispose();
				_endpointConfigurators = null;

				_transportConfigurators.Dispose();
				_transportConfigurators = null;

				_endpoints.Values.Each(endpoint => endpoint.Dispose());

				_endpoints.Dispose();
				_endpoints = null;
			}
			_disposed = true;
		}

		private IEndpoint BuildEndpoint(Uri uri)
		{
			var configurator = BuildEndpointConfigurationAction(uri);

			foreach (var transport in _transportConfigurators.Values)
			{
				IEndpoint endpoint = transport(uri, configurator);
				if (endpoint != null)
					return endpoint;
			}

			throw new ConfigurationException("No transport could handle " + uri);
		}

		private void ConnectTransportConfigurators(IEnumerable<Type> transportTypes)
		{
			const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

			foreach (Type transportType in transportTypes)
			{
				MethodInfo mi = GetConfigureEndpointMethodInfo(transportType, bindingFlags);

				Func<Uri, Action<IEndpointConfigurator>, IEndpoint> invoker = BuildConfigureEndpointLambda(mi);

				_transportConfigurators.Store(transportType, invoker);
			}
		}

		private Action<IEndpointConfigurator> BuildEndpointConfigurationAction(Uri uri)
		{
			Uri key = new Uri(uri.ToString().ToLowerInvariant());

			return x =>
				{
					x.SetSerializer(_defaultSerializer);
					x.SetObjectBuilder(_objectBuilder);

					Action<IEndpointConfigurator> endpointConfigurator;
					if (_endpointConfigurators.TryGetValue(key, out endpointConfigurator))
					{
						endpointConfigurator(x);
					}
				};
		}

		~EndpointFactory()
		{
			Dispose(false);
		}

		private static MethodInfo GetConfigureEndpointMethodInfo(Type transportType, BindingFlags bindingFlags)
		{
			MethodInfo mi = transportType.GetMethod("ConfigureEndpoint", bindingFlags);
			if (mi == null)
				throw new ConfigurationException("The transport does not have a ConfigureEndpoint method: " + transportType.FullName);
			return mi;
		}

		private static Func<Uri, Action<IEndpointConfigurator>, IEndpoint> BuildConfigureEndpointLambda(MethodInfo mi)
		{
			var endpointUri = Expression.Parameter(typeof (Uri), "uri");
			var endpointConfigurator = Expression.Parameter(typeof (Action<IEndpointConfigurator>), "endpointConfigurator");
			var caller = Expression.Call(mi, new[] {endpointUri, endpointConfigurator});
			return Expression.Lambda<Func<Uri, Action<IEndpointConfigurator>, IEndpoint>>(caller, new[] {endpointUri, endpointConfigurator}).Compile();
		}
	}
}