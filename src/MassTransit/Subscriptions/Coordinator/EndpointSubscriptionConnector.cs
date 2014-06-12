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
namespace MassTransit.Subscriptions.Coordinator
{
    using System;
    using Pipeline;

    public interface EndpointSubscriptionConnector
    {
        UnsubscribeAction Connect(Uri endpointUri, string correlationId);
    }

    public class EndpointSubscriptionConnector<TMessage> :
        EndpointSubscriptionConnector
        where TMessage : class
    {
        readonly IServiceBus _bus;

        public EndpointSubscriptionConnector(IServiceBus bus)
        {
            _bus = bus;
        }

        public UnsubscribeAction Connect(Uri endpointUri, string correlationId)
        {
            IEndpoint endpoint = _bus.GetEndpoint(endpointUri);

            return _bus.OutboundPipeline.ConnectEndpoint<TMessage>(endpoint);
        }
    }
}