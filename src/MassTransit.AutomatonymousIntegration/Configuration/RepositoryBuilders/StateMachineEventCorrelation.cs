// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous.RepositoryBuilders
{
    using System;
    using System.Linq.Expressions;


    /// <summary>
    /// A correlation for a data event to the state machine where the default correlation
    /// id is not present or not desired.
    /// </summary>
    /// <typeparam name="TInstance">The state machine instance type</typeparam>
    public interface StateMachineEventCorrelation<TInstance>
        where TInstance : SagaStateMachineInstance
    {
        /// <summary>
        /// The event for this correlation type
        /// </summary>
        Event Event { get; }

        /// <summary>
        /// The correlation expression which can be used to query some storage medium
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        Expression<Func<TInstance, TMessage, bool>> GetCorrelationExpression<TMessage>()
            where TMessage : class;

        /// <summary>
        /// Returns the correlationId that should be used for creating a new saga instance from
        /// this message
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        Guid GetCorrelationId<TMessage>(TMessage message)
            where TMessage : class;

        /// <summary>
        /// Returns the retry limit for the event to support the retry later behavior
        /// for synchronizing correlated events
        /// </summary>
        int RetryLimit { get; }
    }
}