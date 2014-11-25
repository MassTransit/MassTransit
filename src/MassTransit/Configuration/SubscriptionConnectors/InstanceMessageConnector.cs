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
    using System;
    using Magnum.Extensions;
    using Pipeline;
    using Pipeline.Filters;
    using Policies;
    using Util;


    public class InstanceMessageConnector<TConsumer, TMessage> :
        InstanceMessageConnector
        where TConsumer : class
        where TMessage : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _consumeFilter;

        public InstanceMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _consumeFilter = consumeFilter;
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public ConnectHandle Connect(IInboundPipe pipe, object instance, IRetryPolicy retryPolicy)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var consumer = instance as TConsumer;
            if (consumer == null)
            {
                throw new ConsumerException(string.Format("The instance type {0} does not match the consumer type: {1}",
                    TypeMetadataCache<TConsumer>.ShortName, instance.GetType().ToShortTypeName()));
            }

            IPipe<ConsumeContext<TMessage>> instancePipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                x.Retry(retryPolicy);
                x.Filter(new InstanceMessageFilter<TConsumer, TMessage>(consumer, _consumeFilter));
            });

            return pipe.Connect(instancePipe);
        }
    }
}