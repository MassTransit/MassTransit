// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using MassTransit.Topology;
    using Util;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator
        where TMessage : class
    {
        public ServiceBusMessagePublishTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
            : base(entityNameFormatter)
        {
        }

        IServiceBusMessagePublishTopologyConfigurator<T> IServiceBusMessagePublishTopologyConfigurator.GetMessageTopology<T>()
        {
            var result = this as IServiceBusMessagePublishTopologyConfigurator<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public override bool TryGetPublishAddress(Uri baseAddress, TMessage message, out Uri publishAddress)
        {
            var entityName = EntityNameFormatter.FormatEntityName();

            var builder = new UriBuilder(baseAddress)
            {
                Path = entityName
            };

            publishAddress = builder.Uri;
            return true;
        }
    }
}