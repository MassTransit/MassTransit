// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Configuration;
    using Transports;


    public class ServiceBusPublishEndpoint :
        PublishEndpoint
    {
        readonly IServiceBusHost _host;
        readonly IMessageNameFormatter _nameFormatter;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceBusPublishEndpoint(IServiceBusHost host, ISendEndpointProvider sendEndpointProvider)
        {
            _host = host;
            _sendEndpointProvider = sendEndpointProvider;
            _nameFormatter = host.MessageNameFormatter;
        }

        protected override async Task<IEnumerable<ISendEndpoint>> GetEndpoints(Type messageType)
        {
            Uri address = _nameFormatter.GetTopicAddress(_host, messageType);

            return new[] {await _sendEndpointProvider.GetSendEndpoint(address)};
        }
    }
}