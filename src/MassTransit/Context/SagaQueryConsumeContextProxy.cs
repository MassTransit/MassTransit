// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes.Payloads;
    using Saga;


    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaQueryConsumeContextProxy<TSaga, TMessage> :
        ConsumeContextProxy<TMessage>,
        SagaQueryConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly ISagaQuery<TSaga> _query;

        public SagaQueryConsumeContextProxy(ConsumeContext<TMessage> context, ISagaQuery<TSaga> query)
            : base(context)
        {
            _query = query;
        }

        public SagaQueryConsumeContextProxy(ConsumeContext<TMessage> context, IPayloadCache payloadCache, ISagaQuery<TSaga> query)
            : base(context, payloadCache)
        {
            _query = query;
        }

        ISagaQuery<TSaga> SagaQueryConsumeContext<TSaga, TMessage>.Query => _query;
    }
}