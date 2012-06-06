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
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Pipeline;
    using Util;

    public class InterceptingConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly Action<Action> _interceptor;

        public InterceptingConsumerFactory([NotNull] IConsumerFactory<TConsumer> consumerFactory, [NotNull] Action<Action> interceptor)
        {
            if (consumerFactory == null)
                throw new ArgumentNullException("consumerFactory");
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            _consumerFactory = consumerFactory;
            _interceptor = interceptor;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(IConsumeContext<TMessage> context,
            InstanceHandlerSelector<TConsumer, TMessage> selector)
            where TMessage : class
        {
            yield return x =>
                {
                    _interceptor(() =>
                        {
                            foreach (var consumer in _consumerFactory.GetConsumer(context, selector))
                            {
                                consumer(x);
                            }
                        });
                };
        }
    }
}