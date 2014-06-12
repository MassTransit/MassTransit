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
namespace MassTransit.Testing.TestDecorators
{
    using System;
    using System.Threading.Tasks;


    public class ConsumerFactoryTestDecorator<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly ReceivedMessageListImpl _received;

        public ConsumerFactoryTestDecorator(IConsumerFactory<TConsumer> consumerFactory,
            ReceivedMessageListImpl received)
        {
            _consumerFactory = consumerFactory;
            _received = received;
        }

        Task IAsyncConsumerFactory<TConsumer>.GetConsumer<TMessage>(ConsumeContext<TMessage> consumeContext,
            ConsumerFactoryCallback<TConsumer, TMessage> callback)
        {
            return _consumerFactory.GetConsumer(consumeContext, async (consumer, context) =>
            {
                var received = new ReceivedMessageImpl<TMessage>(context);

                try
                {
                    await callback(consumer, context);
                }
                catch (Exception ex)
                {
                    received.SetException(ex);
                }
                finally
                {
                    _received.Add(received);
                }
            });
        }
    }
}