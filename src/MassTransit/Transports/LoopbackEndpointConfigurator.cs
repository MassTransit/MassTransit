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
    using Configuration;
    using Exceptions;
    using Internal;
    using Magnum;

    public class LoopbackEndpointConfigurator :
        EndpointConfiguratorBase
    {
        static readonly EndpointDefaults _defaults = new EndpointDefaults();

        public IEndpoint New(Action<IEndpointConfigurator> action)
        {
            action(this);

            Guard.AgainstNull(Uri, "No Uri was specified for the endpoint");
            if (MessageSerializer == null)
                Guard.AgainstNull(SerializerType, "No serializer type was specified for the endpoint");

            var settings = new CreateEndpointSettings(Uri)
                {
                    Serializer = GetSerializer(),
                    CreateIfMissing = _defaults.CreateMissingQueues,
                    PurgeExistingMessages = _defaults.PurgeOnStartup,
                    Transactional = _defaults.CreateTransactionalQueues,
                };

            try
            {
                Guard.AgainstNull(settings.Address, "An address for the endpoint must be specified");
                Guard.AgainstNull(settings.ErrorAddress, "An error address for the endpoint must be specified");
                Guard.AgainstNull(settings.Serializer, "A message serializer for the endpoint must be specified");

                var tf = new LoopbackTransportFactory();
                var transport = tf.New(settings.ToTransportSettings());

                tf.PurgeExistingMessagesIfRequested(settings);

                var errorSettings = new CreateEndpointSettings(settings.ErrorAddress, settings);
                ITransport errorTransport = tf.New(errorSettings.ToTransportSettings());

                var endpoint = new Endpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create loopback endpoint", ex);
            }
        }

     
        public static void Defaults(Action<IEndpointDefaults> configureDefaults)
        {
            configureDefaults(_defaults);
        }
    }
}