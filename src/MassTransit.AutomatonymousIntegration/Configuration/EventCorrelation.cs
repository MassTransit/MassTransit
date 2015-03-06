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
    using MassTransit;
    using MassTransit.Saga;


    public interface EventCorrelation<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        Event Event { get; }

        /// <summary>
        /// The data type for the event
        /// </summary>
        Type DataType { get; }
    }


    public interface EventCorrelation<TInstance, TData> :
        EventCorrelation<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        new Event<TData> Event { get; }

        ISagaLocator<TData> GetSagaLocator(ISagaRepository<TInstance> sagaRepository);

        /// <summary>
        /// Attempt to return the correlationId of the saga from the message
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="context">The event context</param>
        /// <returns>True if the correlationId is available, otherwise false</returns>
        Guid GetCorrelationId(ConsumeContext<TData> context);

        /// <summary>
        /// Returns the saga policy for the event correlation
        /// </summary>
        /// <value></value>
        ISagaPolicy<TInstance, TData> Policy { get; }
    }
}