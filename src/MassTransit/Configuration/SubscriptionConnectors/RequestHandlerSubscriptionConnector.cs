// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using Pipeline;
    using Pipeline.Configuration;
    using Pipeline.Sinks;

    public class RequestHandlerSubscriptionConnector<TMessage>
        where TMessage : class
    {
        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, string requestId,
                                         HandlerSelector<TMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler", "The handler cannot be null.");

            var correlatedConfigurator = new InboundRequestMessageRouterConfigurator(configurator.Pipeline);

            RequestMessageRouter<IConsumeContext<TMessage>, TMessage> router =
                correlatedConfigurator.FindOrCreate<TMessage>();

            var sink = new InstanceMessageSink<TMessage>(MultipleHandlerSelector.ForHandler(handler));

            UnsubscribeAction result = router.Connect(requestId, sink);

            return () => result() && (router.SinkCount(requestId) == 0); // && remove();
        }
    }
}