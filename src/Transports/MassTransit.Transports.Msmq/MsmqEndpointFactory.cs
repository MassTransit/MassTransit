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
namespace MassTransit.Transports.Msmq
{
    using System;
    using Exceptions;
    using Magnum;
    using Serialization;

    public static class MsmqEndpointFactory
    {
        public static IEndpoint New(IEndpointAddress address, IMessageSerializer serializer)
        {
            return New(new CreateMsmqEndpointSettings(address)
                {
                    Serializer = serializer,
                });
        }

        public static IEndpoint New(CreateMsmqEndpointSettings settings)
        {
            try
            {
                Guard.AgainstNull(settings.Address, "An address for the endpoint must be specified");
                Guard.AgainstNull(settings.ErrorAddress, "An error address for the endpoint must be specified");
                Guard.AgainstNull(settings.Serializer, "A message serializer for the endpoint must be specified");

                var transport = MsmqTransportFactory.New(settings);
                
				// TODO Does this need to be a bus concern?
                PurgeExistingMessagesIfRequested(settings);

                var errorSettings = new CreateMsmqTransportSettings(settings.ErrorAddress, settings);
				if(transport.Address.IsTransactional)
					settings.Transactional = true;

                ITransport errorTransport = MsmqTransportFactory.New(errorSettings);

                var endpoint = new MsmqEndpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create MSMQ endpoint", ex);
            }
        }

        private static void PurgeExistingMessagesIfRequested(CreateMsmqEndpointSettings settings)
        {
            if(settings.Address.IsLocal && settings.PurgeExistingMessages)
            {
                MsmqEndpointManagement.Manage(settings.Address, x => x.Purge());
            }
        }
    }
}