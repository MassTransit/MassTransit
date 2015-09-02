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
    using System;


    /// <summary>
    /// Holds the state of a scheduled message
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface Schedule<in TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// The name of the scheduled message
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The delay for the message
        /// </summary>
        TimeSpan Delay { get; }

        /// <summary>
        /// Return the TokenId for the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Guid? GetTokenId(TInstance instance);

        /// <summary>
        /// Set the token ID on the Instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="tokenId"></param>
        void SetTokenId(TInstance instance, Guid? tokenId);
    }


    /// <summary>
    /// Holds the state of a scheduled message
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface Schedule<in TInstance, TMessage> :
        Schedule<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// This event is raised when the scheduled message is received. If a previous message
        /// was rescheduled, this event is filtered so that only the most recently scheduled
        /// message is allowed.
        /// </summary>
        Event<TMessage> Received { get; set; }

        /// <summary>
        /// This event is raised when any message is directed at the state machine, but it is 
        /// not filtered to the currently scheduled event. So outdated or original events may
        /// be raised.
        /// </summary>
        Event<TMessage> AnyReceived { get; set; }
    }
}