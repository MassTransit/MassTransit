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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Exceptions;
    using Internal;

    public class EndpointFactory :
        IEndpointFactory
    {
        readonly IEnumerable<ITransportFactory> _factories;

        public EndpointFactory(IEnumerable<ITransportFactory> factories)
        {
            _factories = factories;
        }

        public IEndpoint BuildEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
        {
            foreach (var factory in _factories)
            {
                try
                {

                    if (uri.Scheme.ToLowerInvariant() == factory.Scheme)
                    {
                        var epc = new EndpointConfigurator();
                        epc.SetUri(uri);
                        var s = epc.New(configurator);

                        var transport = factory.BuildLoopback(s.Normal);
                        var errorTransport = factory.BuildLoopback(s.Error);

                        var endpoint = new Endpoint(transport.Address, epc.GetSerializer(), transport,
                                                    errorTransport);

                        return endpoint;
                    }
                }
                catch (Exception ex)
                {
                    throw new EndpointException(uri, "Error", ex);
                }
            }

            throw new ConfigurationException("No transport could handle: '{0}'".FormatWith(uri));
        }
    }
}