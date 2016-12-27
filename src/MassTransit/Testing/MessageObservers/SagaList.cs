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
namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Saga;


    public class SagaList<T> :
        MessageList<ISagaInstance<T>>,
        ISagaList<T>
        where T : class, ISaga
    {
        public SagaList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public IEnumerable<ISagaInstance<T>> Select(Func<T, bool> filter)
        {
            return base.Select(x => true);
        }

        public T Contains(Guid sagaId)
        {
            return Select(x => x.Saga.CorrelationId == sagaId).Select(x => x.Saga).FirstOrDefault();
        }

        public void Add(SagaConsumeContext<T> context)
        {
            Add(new SagaInstance<T>(context.Saga), context.Saga.CorrelationId);
        }


        class SagaEqualityComparer :
            IEqualityComparer<ISagaInstance<T>>
        {
            public bool Equals(ISagaInstance<T> x, ISagaInstance<T> y)
            {
                return Equals(x.Saga.CorrelationId, y.Saga.CorrelationId);
            }

            public int GetHashCode(ISagaInstance<T> message)
            {
                return message.Saga.CorrelationId.GetHashCode();
            }
        }
    }
}