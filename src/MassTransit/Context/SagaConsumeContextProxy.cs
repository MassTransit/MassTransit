// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System.Threading.Tasks;
    using Saga;
    using Util;


    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaConsumeContextProxy<TSaga, TMessage> :
        ConsumeContextProxy<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga, TMessage> _sagaContext;

        public SagaConsumeContextProxy(ConsumeContext<TMessage> context, SagaConsumeContext<TSaga, TMessage> sagaContext)
            : base(context)
        {
            _sagaContext = sagaContext;
        }

        public TSaga Saga => _sagaContext.Saga;

        public SagaConsumeContext<TSaga, T> PopContext<T>()
            where T : class
        {
            var context = this as SagaConsumeContext<TSaga, T>;
            if (context == null)
                throw new ContextException($"The ConsumeContext<{TypeMetadataCache<TMessage>.ShortName}> could not be cast to {TypeMetadataCache<T>.ShortName}");

            return context;
        }

        public Task SetCompleted()
        {
            return _sagaContext.SetCompleted();
        }

        public bool IsCompleted => _sagaContext.IsCompleted;
    }
}