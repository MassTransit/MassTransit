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
    using Internal;

    public class EndpointFactory :
        IEndpointFactory
    {
        IEnumerable<ITransportFactory> _factories;

        public IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
        {
            var address = new EndpointAddress(uri);
            var epc = new EndpointConfigurator();
            epc.SetUri(uri);
            configurator(epc);

            var ep = new CreateEndpointSettings(address);


            var s = ep.ToTransportSettings();
            var e = ep.ToTransportSettings();

            foreach (var fac in _factories)
            {
                if (uri.Scheme.ToLowerInvariant() == fac.Scheme)
                {
                    var transport = fac.New(s);
                    var errorTransport = fac.New(e);

                    var endpoint = new Endpoint(address, null, transport, errorTransport);


                    return endpoint;
                }
            }

            return null;
        }
    }
}