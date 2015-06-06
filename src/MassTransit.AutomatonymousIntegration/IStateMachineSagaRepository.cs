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
namespace Automatonymous
{
    using System;
    using System.Linq.Expressions;
    using MassTransit.Saga;


    public interface IStateMachineSagaRepository<TInstance> :
        ISagaRepository<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// Requests a correlation expression and identifier selector for the
        /// event type.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="event">The event requested</param>
        /// <param name="correlationExpression">The expression to correlate a message to a state machine instance</param>
        /// <param name="correlationIdSelector">The function to return a correlation id from the message</param>
        /// <returns>True if a correlation is specified, otherwise false</returns>
        bool TryGetCorrelationExpressionForEvent<TData>(Event<TData> @event,
            out Expression<Func<TInstance, TData, bool>> correlationExpression,
            out Func<TData, Guid> correlationIdSelector)
            where TData : class;

        /// <summary>
        /// Returns the completed expression for the state machine, so that completed
        /// instances can be removed from the repository.
        /// </summary>
        /// <returns>The completed expression</returns>
        Expression<Func<TInstance, bool>> GetCompletedExpression();
    }
}