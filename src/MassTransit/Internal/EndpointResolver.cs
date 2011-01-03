// Copyright 2007-2011 The Apache Software Foundation.
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
    using Configuration;
    using Exceptions;
    using Magnum;
    using Magnum.Threading;
    using Transports;

    public class EndpointResolver :
        IEndpointResolver
    {
        static readonly IEndpoint _null = new NullEndpoint();
        readonly Type _defaultSerializer;
        volatile bool _disposed;
        ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>> _endpointConfigurators;
        ReaderWriterLockedDictionary<Uri, IEndpoint> _endpoints;
        readonly IEnumerable<IEndpointFactory> _factories;

        public EndpointResolver(Type defaultSerializer,
                                IEnumerable<IEndpointFactory> transportFactories,
                                IEnumerable<KeyValuePair<Uri, Action<IEndpointConfigurator>>> endpointConfigurators)
        {
            _endpointConfigurators = new ReaderWriterLockedDictionary<Uri, Action<IEndpointConfigurator>>(endpointConfigurators);
            _endpoints = new ReaderWriterLockedDictionary<Uri, IEndpoint>();

            _defaultSerializer = defaultSerializer;

            _factories = transportFactories;
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
                throw new ArgumentException("The endpoint Uri is invalid: '{0}'".FormatWith(uriString), ex);
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
                throw new ConfigurationException("An error occurred retrieving the endpoint for '{0}'".FormatWith(uri), ex);
            }
        }

        IEndpoint BuildEndpoint(Uri uri)
        {
            var configurator = BuildEndpointConfiguration(uri);

            foreach (var fact in _factories)
            {
                IEndpoint ep = fact.BuildEndpoint(uri, configurator);
                if (ep != null)
                    return ep;
            }

            throw new ConfigurationException("No transport could handle: '{0}'".FormatWith(uri));
        }

        Action<IEndpointConfigurator> BuildEndpointConfiguration(Uri uri)
        {
            Uri key = new Uri(uri.ToString().ToLowerInvariant());

            return x =>
            {
                x.SetSerializer(_defaultSerializer);

                Action<IEndpointConfigurator> endpointConfigurator;
                if (_endpointConfigurators.TryGetValue(key, out endpointConfigurator))
                {
                    endpointConfigurator(x);
                }
            };
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _endpointConfigurators.Dispose();
                _endpointConfigurators = null;

                _endpoints.Values.Each(endpoint => endpoint.Dispose());

                _endpoints.Dispose();
                _endpoints = null;
            }
            _disposed = true;
        }

        ~EndpointResolver()
        {
            Dispose(false);
        }
    }
}