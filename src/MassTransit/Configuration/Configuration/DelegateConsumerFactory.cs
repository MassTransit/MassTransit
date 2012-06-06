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
    using Exceptions;
    using Pipeline;


    public class DelegateConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly Func<TConsumer> _factoryMethod;

        public DelegateConsumerFactory(Func<TConsumer> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
            IConsumeContext<TMessage> context, InstanceHandlerSelector<TConsumer, TMessage> selector)
            where TMessage : class
        {
            TConsumer consumer = _factoryMethod();
            if (consumer == null)
                throw new ConfigurationException(string.Format("Unable to resolve consumer type '{0}'.",
                    typeof(TConsumer)));

            try
            {
                foreach (var handler in selector(consumer, context))
                {
                    yield return handler;
                }
            }
            finally
            {
                var disposable = consumer as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }
}