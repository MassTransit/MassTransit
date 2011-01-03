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
namespace MassTransit.Transports
{
    using System;
    using Configuration;
    using Exceptions;
    using Internal;
    using Magnum;
    using Serialization;

    public class MulticastUdpEndpointConfigurator :
        EndpointConfiguratorBase
    {
        public static IEndpoint New(Action<IEndpointConfigurator> action)
        {
            var configurator = new MulticastUdpEndpointConfigurator();

            action(configurator);

            return configurator.Create();
        }

        private IEndpoint Create()
        {
            Guard.AgainstNull(Uri, "No Uri was specified for the endpoint");
            Guard.AgainstNull(SerializerType, "No serializer type was specified for the endpoint");

            IEndpoint endpoint = New(new CreateEndpointSettings(Uri)
                {
                    Serializer = GetSerializer(),
                });

            return endpoint;
        }

        public IEndpoint New(IEndpointAddress address, IMessageSerializer serializer)
        {
            return New(new CreateEndpointSettings(address)
                {
                    Serializer = serializer,
                });
        }

        public IEndpoint New(CreateEndpointSettings settings)
        {
            try
            {
                Guard.AgainstNull(settings.Address, "An address for the endpoint must be specified");
                Guard.AgainstNull(settings.ErrorAddress, "An error address for the endpoint must be specified");
                Guard.AgainstNull(settings.Serializer, "A message serializer for the endpoint must be specified");

                var transport = new MulticastUdpTransport(settings.Address);

                var errorTransport = new NullTransport(settings.ErrorAddress);

                var endpoint = new Endpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create multicast endpoint", ex);
            }
        }

    }
}