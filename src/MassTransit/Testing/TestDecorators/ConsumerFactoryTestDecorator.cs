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
    using Pipeline;


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

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return _consumerFactory.Send(context, new TestDecoratorPipe<TMessage>(_received, next));
        }


        class TestDecoratorPipe<T> :
            IPipe<ConsumerConsumeContext<TConsumer, T>>
            where T : class
        {
            readonly IPipe<ConsumerConsumeContext<TConsumer, T>> _next;
            readonly ReceivedMessageListImpl _received;

            public TestDecoratorPipe(ReceivedMessageListImpl received, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
            {
                _received = received;
                _next = next;
            }

            public async Task Send(ConsumerConsumeContext<TConsumer, T> context)
            {
                var received = new ReceivedMessageImpl<T>(context);

                try
                {
                    await _next.Send(context);
                }
                catch (Exception ex)
                {
                    received.SetException(ex);
                }
                finally
                {
                    _received.Add(received);
                }
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this, x => _next.Inspect(x));
            }
        }
    }
}