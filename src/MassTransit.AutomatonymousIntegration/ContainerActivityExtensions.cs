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
namespace Automatonymous
{
    using System;
    using Activities;
    using Binders;


    public static class ContainerActivityExtensions
    {
        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="binder"></param>
        /// <param name="activityFactory"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Activity<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Func<IStateMachineActivitySelector<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            var selector = new StateMachineActivitySelector<TInstance, TData>(binder);

            return activityFactory(selector);
        }

        /// <summary>
        /// Adds an activity to the state machine that is resolved from the container, rather than being initialized directly.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="binder"></param>
        /// <param name="activityFactory"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Activity<TInstance>(this EventActivityBinder<TInstance> binder,
            Func<IStateMachineActivitySelector<TInstance>, EventActivityBinder<TInstance>> activityFactory)
            where TInstance : class, SagaStateMachineInstance
        {
            var selector = new StateMachineActivitySelector<TInstance>(binder);

            return activityFactory(selector);
        }
    }
}