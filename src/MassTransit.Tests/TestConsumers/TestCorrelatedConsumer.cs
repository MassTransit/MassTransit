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
namespace MassTransit.Tests.TestConsumers
{
    using System;


    public class TestCorrelatedConsumer<TMessage, TKey> :
        TestConsumerBase<TMessage>,
        Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly TKey _correlationId;

        public TestCorrelatedConsumer(TKey correlationId)
        {
            _correlationId = correlationId;
        }

        public override void Consume(TMessage message)
        {
            if (_correlationId.Equals(message.CorrelationId))
                base.Consume(message);
        }
    }
}