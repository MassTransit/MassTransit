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
namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;
    using Util;

    public class DataEventBinder<TSaga, TMessage> :
        EventBinder<TSaga, TMessage>
    {
        Func<TMessage, Guid> _correlationIdSelector;
        Expression<Func<TSaga, TMessage, bool>> _expression;

        public EventBinder<TSaga, TMessage> By([NotNull] Expression<Func<TSaga, TMessage, bool>> expression)
        {
            _expression = expression;

            return this;
        }

        public EventBinder<TSaga, TMessage> UseId([NotNull] Func<TMessage, Guid> correlationIdSelector)
        {
            _correlationIdSelector = correlationIdSelector;

            return this;
        }

        public Expression<Func<TSaga, T, bool>> GetBindExpression<T>()
        {
            var self = this as DataEventBinder<TSaga, T>;
            if (self == null)
                throw new ArgumentException("Message type " + typeof(T).Name + " is not assignable to "
                                            + typeof(TMessage).Name);

            return self._expression;
        }

        public Func<T, Guid> GetCorrelationIdSelector<T>()
        {
            var self = this as DataEventBinder<TSaga, T>;
            if (self == null)
                throw new ArgumentException("Message type " + typeof(T).Name + " is not assignable to "
                                            + typeof(TMessage).Name);

            return self._correlationIdSelector;
        }
    }
}