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
namespace MassTransit.Saga
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Logging;
    using Util;


    /// <summary>
    /// Creates a saga instance using the default factory method
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class FactoryMethodSagaFactory<TSaga, TMessage> :
        ISagaFactory<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<FactoryMethodSagaFactory<TSaga, TMessage>>();
        readonly SagaFactoryMethod<TSaga, TMessage> _factoryMethod;

        public FactoryMethodSagaFactory(SagaFactoryMethod<TSaga, TMessage> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public TSaga Create(ConsumeContext<TMessage> context)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The correlationId was not present and the saga could not be created", typeof(TSaga), typeof(TMessage));

            return _factoryMethod(context);
        }

        public Task Send(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The correlationId was not present and the saga could not be created", typeof(TSaga), typeof(TMessage));

            TSaga instance = _factoryMethod(context);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("SAGA:{0}:{1} Created {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<TMessage>.ShortName);

            var proxy = new NewSagaConsumeContext<TSaga, TMessage>(context, instance);

            return next.Send(proxy);
        }
    }
}