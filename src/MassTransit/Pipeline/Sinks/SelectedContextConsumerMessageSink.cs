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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Collections.Generic;
    using Magnum.Extensions;

    public class SelectedContextConsumerMessageSink<TConsumer, TMessage> :
        IPipelineSink<IConsumeContext<TMessage>>
        where TMessage : class
        where TConsumer : class, Consumes<IConsumeContext<TMessage>>.Selected
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public SelectedContextConsumerMessageSink(IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> Enumerate(IConsumeContext<TMessage> context)
        {
            return _consumerFactory.GetConsumer(context, Selector);
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this);
        }

        IEnumerable<Action<IConsumeContext<TMessage>>> Selector(TConsumer instance,
                                                                IConsumeContext<TMessage> messageContext)
        {
            bool accept = instance.Accept(messageContext);

            if (accept)
            {
                yield return context =>
                    {
                        context.BaseContext.NotifyConsume(context, typeof (TConsumer).ToShortTypeName(), null);

                        instance.Consume(context);
                    };
            }
        }
    }
}