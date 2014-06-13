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
namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;


    public class InterceptingConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly ConsumerFactoryInterceptor<TConsumer> _interceptor;

        public InterceptingConsumerFactory(IConsumerFactory<TConsumer> consumerFactory,
            ConsumerFactoryInterceptor<TConsumer> interceptor)
        {
            if (consumerFactory == null)
                throw new ArgumentNullException("consumerFactory");
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            _consumerFactory = consumerFactory;
            _interceptor = interceptor;
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return _consumerFactory.Send(context, new TestDecoratorPipe<TMessage>(_interceptor, next));
        }


        class TestDecoratorPipe<T> :
            IPipe<ConsumeContext<TConsumer, T>>
            where T : class
        {
            readonly ConsumerFactoryInterceptor<TConsumer> _interceptor;
            readonly IPipe<ConsumeContext<TConsumer, T>> _next;

            public TestDecoratorPipe(ConsumerFactoryInterceptor<TConsumer> interceptor,
                IPipe<ConsumeContext<TConsumer, T>> next)
            {
                _interceptor = interceptor;
                _next = next;
            }

            public Task Send(ConsumeContext<TConsumer, T> context)
            {
                return _interceptor(context.Item1, context, () => _next.Send(context));
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this, (x, p) => _next.Inspect(x));
            }
        }
    }
}