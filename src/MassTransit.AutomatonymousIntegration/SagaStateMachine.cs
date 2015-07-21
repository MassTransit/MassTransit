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
namespace Automatonymous
{
    using System.Collections.Generic;


    public interface SagaStateMachine<TInstance> :
        StateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// Returns the event correlations for the state machine
        /// </summary>
        IEnumerable<EventCorrelation> Correlations { get; }

        /// <summary>
        /// Returns true if the saga state machine instance is complete and can be removed from the repository
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool IsCompleted(TInstance instance);
    }
}