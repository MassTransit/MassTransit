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
    using Binders;


    public interface IStateMachineActivitySelector<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        /// <summary>
        /// An activity which accepts the instance and data from the event
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <returns></returns>
        EventActivityBinder<TInstance, TData> OfType<TActivity>()
            where TActivity : Activity<TInstance, TData>;

        /// <summary>
        /// An activity that only accepts the instance, and does not require the event data
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <returns></returns>
        EventActivityBinder<TInstance, TData> OfInstanceType<TActivity>()
            where TActivity : Activity<TInstance>;
    }


    public interface IStateMachineActivitySelector<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// An activity which accepts the instance and data from the event
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <returns></returns>
        EventActivityBinder<TInstance> OfType<TActivity>()
            where TActivity : Activity<TInstance>;
    }
}