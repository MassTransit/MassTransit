// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests.StateMachine
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class StateMachineBase<T> where T : StateMachineBase<T>
    {
        public StateMachineBase()
        {
            Current = Initial;
        }

        public static State<T> Initial { get; set; }

        public State<T> Current { get; private set; }

        public void Handle(StateEvent<T> stateEvent)
        {
            State<T> newState = Current.Handle(stateEvent);
            if (newState == Current)
                return;
            
            Current.Leave((T)this);

            Current = newState;

            Current.Enter((T)this);
        }

        public static StateBuilder<T> Define(Expression<Func<State<T>>> func)
        {
            State<T> state = SetProperty<State<T>>(func, x => new State<T>(x.Name));

            return new StateBuilder<T>(state);
        }

        public static StateEvent<T> Define(Expression<Func<StateEvent<T>>> func)
        {
            return SetProperty<StateEvent<T>>(func, x => new StateEvent<T>(x.Name));
        }

        public static TValue SetProperty<TValue>(Expression<Func<TValue>> func, Func<PropertyInfo, TValue> getInstance)
        {
            MemberExpression mex = func.Body as MemberExpression;
            if (mex == null)
                throw new ArgumentException("The function must be a property lambda");

            PropertyInfo property = ((PropertyInfo)mex.Member);

            var value = Expression.Parameter(typeof(TValue), "value");
            var action = Expression.Lambda<Action<TValue>>(Expression.Call(property.GetSetMethod(), value), new[] { value }).Compile();

            TValue propertyValue = getInstance(property);
            action(propertyValue);

            return propertyValue;
        }

    }
}