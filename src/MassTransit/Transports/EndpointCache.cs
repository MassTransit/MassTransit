// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Diagnostics.Introspection;
    using Exceptions;
    using Logging;
    using Magnum;
    using Magnum.Caching;


    public class EndpointCache :
        IEndpointCache
    {
        static readonly ILog _log = Logger.Get(typeof(EndpointCache));

        readonly IEndpointFactory _endpointFactory;
        readonly Cache<Uri, IEndpoint> _endpoints;
        bool _disposed;

        public EndpointCache(IEndpointFactory endpointFactory)
        {
            _endpointFactory = endpointFactory;

            _endpoints = new ConcurrentCache<Uri, IEndpoint>();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IEndpoint GetEndpoint(Uri uri)
        {
            if (_disposed)
                throw new ObjectDisposedException("The endpoint resolver has been disposed");

            Guard.AgainstNull(uri, "uri", "Uri cannot be null");

            try
            {
                var key = new Uri(uri.ToString().ToLowerInvariant());

                return _endpoints.Get(key, _ => _endpointFactory.CreateEndpoint(uri));
            }
            catch (TransportException)
            {
                throw;
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

        public void Inspect(DiagnosticsProbe probe)
        {
            _endpointFactory.Inspect(probe);
        }

        public void Clear()
        {
            IEndpoint[] endpoints = _endpoints.GetAll();
            _endpoints.Clear();

            foreach (IEndpoint endpoint in endpoints)
            {
                try
                {
                    endpoint.Dispose();
                }
                catch (Exception ex)
                {
                    _log.Error("An exception was thrown while disposing of an endpoint: " + endpoint.Address, ex);
                }
            }

            _endpoints.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Clear();

                _endpointFactory.Dispose();
            }

            _disposed = true;
        }
    }
}