// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using Pipeline;
    using Pipeline.Configuration;
    using Pipeline.Sinks;


    public class HandlerSubscriptionConnector<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Connects a handler selector through an 'instance message sink' and makes sure that
        /// the message router is wired up to route messages to this handler.
        /// </summary>
        /// <param name="configurator">The inbound pipeline configurator</param>
        /// <param name="handler">The handler to subscribe.</param>
        /// <returns>An action that can be called to unsubscribe the handler.</returns>
        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, HandlerSelector<TMessage> handler)
        {
            var routerConfigurator = new InboundMessageRouterConfigurator(configurator.Pipeline);

            MessageRouter<IConsumeContext<TMessage>> router = routerConfigurator.FindOrCreate<TMessage>();

            var sink = new InstanceMessageSink<TMessage>(MultipleHandlerSelector.ForHandler(handler));

            // connect the router
            UnsubscribeAction result = router.Connect(sink);

            // and notify the subscription
            UnsubscribeAction remove = configurator.SubscribedTo<TMessage>();

            // remove the subscription when there are no further sinks for TMessage.
            return () => result() && (router.SinkCount == 0) && remove();
        }
    }
}