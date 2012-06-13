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
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using Magnum.Caching;
    using Magnum.StateMachine;

    [DebuggerDisplay("{CurrentState} - {typeof(T).Name}")]
    public class SagaStateMachine<T> :
        StateMachine<T>
        where T : SagaStateMachine<T>
    {
        static readonly Cache<Event, EventBinder<T>> _binders = new DictionaryCache<Event, EventBinder<T>>();
        static Expression<Func<T, bool>> _completedExpression = x => false;

        protected SagaStateMachine()
        {
        }

        public SagaStateMachine(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static bool TryGetEventBinder(Event targetEvent, out EventBinder<T> expression)
        {
            if (_binders.Has(targetEvent))
            {
                expression = _binders[targetEvent];
                return true;
            }

            expression = null;
            return false;
        }

        public static Expression<Func<T, bool>> GetCompletedExpression()
        {
            return _completedExpression;
        }

        protected static void RemoveWhen(Expression<Func<T, bool>> expression)
        {
            _completedExpression = expression;
        }

        protected static EventBinder<T, V> Correlate<V>(Event<V> targetEvent)
        {
            return (EventBinder<T, V>)_binders.Get(targetEvent, x => new DataEventBinder<T, V>());
        }
    }
}