// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.Events
{
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using InternalMessages;


    public class RoutingSlipEventPublisher :
        IRoutingSlipEventPublisher
    {
        readonly IPublishEndpoint _publishEndpoint;
        readonly RoutingSlip _routingSlip;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public RoutingSlipEventPublisher(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint,
            RoutingSlip routingSlip)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _routingSlip = routingSlip;
        }

        public Task Publish(RoutingSlipCompleted message)
        {
            return PublishEvent(message, RoutingSlipEvents.Completed);
        }

        public Task Publish(RoutingSlipFaulted message)
        {
            return PublishEvent(message, RoutingSlipEvents.Faulted);
        }

        public Task Publish(RoutingSlipActivityCompleted message)
        {
            return PublishEvent(message, RoutingSlipEvents.ActivityCompleted);
        }

        public Task Publish(RoutingSlipActivityFaulted message)
        {
            return PublishEvent(message, RoutingSlipEvents.ActivityFaulted);
        }

        public Task Publish(RoutingSlipActivityCompensated message)
        {
            return PublishEvent(message, RoutingSlipEvents.ActivityCompensated);
        }

        public Task Publish(CompensationFailed message)
        {
            return PublishEvent(message, RoutingSlipEvents.ActivityCompensationFailed | RoutingSlipEvents.CompensationFailed);
        }

        async Task PublishEvent<T>(T message, RoutingSlipEvents eventFlag)
            where T : class
        {
            if (_routingSlip.Subscriptions.Any())
            {
                foreach (Subscription subscription in _routingSlip.Subscriptions)
                {
                    if (subscription.Events == RoutingSlipEvents.All || subscription.Events.HasFlag(eventFlag))
                    {
                        ISendEndpoint endpoint = await _sendEndpointProvider.GetSendEndpoint(subscription.Address);

                        T subscriptionMessage = RoutingSlipEventFilterCache.Filter(message, subscription.Include);

                        await endpoint.Send(subscriptionMessage);
                    }
                }
            }
            else
                await _publishEndpoint.Publish(message);
        }
    }
}