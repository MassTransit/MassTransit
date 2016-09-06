// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Pipeline;


    public class TestConsumerFactoryDecorator<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly ReceivedMessageList _received;

        public TestConsumerFactoryDecorator(IConsumerFactory<TConsumer> consumerFactory,
            ReceivedMessageList received)
        {
            _consumerFactory = consumerFactory;
            _received = received;
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return _consumerFactory.Send(context, new TestDecoratorPipe<TMessage>(_received, next));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("testDecorator");

             _consumerFactory.Probe(scope);
        }


        class TestDecoratorPipe<TMessage> :
            IPipe<ConsumerConsumeContext<TConsumer, TMessage>>
            where TMessage : class
        {
            readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _next;
            readonly ReceivedMessageList _received;

            public TestDecoratorPipe(ReceivedMessageList received, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            {
                _received = received;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context)
            {
                try
                {
                    await _next.Send(context).ConfigureAwait(false);

                    _received.Add(context);
                }
                catch (Exception ex)
                {
                    _received.Add(context, ex);
                }
            }
        }
    }
}