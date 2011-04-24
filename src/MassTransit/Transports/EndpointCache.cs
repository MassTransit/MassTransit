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
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum;
	using Magnum.Threading;

	public class EndpointCache :
		IEndpointCache
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (EndpointCache));
		static IEndpoint _null;

		readonly IEndpointFactory _endpointFactory;
		volatile bool _disposed;
		ReaderWriterLockedDictionary<Uri, IEndpoint> _endpoints;

		public EndpointCache(IEndpointFactory endpointFactory)
		{
			_endpointFactory = endpointFactory;

			_endpoints = new ReaderWriterLockedDictionary<Uri, IEndpoint>();
		}

		public static IEndpoint Null
		{
			get { return _null ?? (_null = new NullEndpoint()); }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEndpoint GetEndpoint(Uri uri)
		{
			if (_disposed)
				throw new ObjectDisposedException("The endpoint resolver has been disposed");

			Guard.AgainstNull(uri, "uri", "Uri cannot be null");

			try
			{
				var key = new Uri(uri.ToString().ToLowerInvariant());

				return _endpoints.Retrieve(key, () => _endpointFactory.CreateEndpoint(uri));
			}
			catch (EndpointException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ConfigurationException("An exception was thrown retrieving the endpoint:" + uri, ex);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				foreach (var endpoint in _endpoints)
				{
					try
					{
						endpoint.Value.Dispose();
					}
					catch (Exception ex)
					{
						_log.Error("An exception was thrown while disposing of an endpoint: " + endpoint.Key);
					}
				}

				_endpoints.Dispose();
				_endpoints = null;
			}

			_disposed = true;
		}


		~EndpointCache()
		{
			Dispose(false);
		}
	}
}