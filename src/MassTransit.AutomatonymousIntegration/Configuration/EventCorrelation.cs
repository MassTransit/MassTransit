// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public interface EventCorrelation :
        ISpecification
    {
        /// <summary>
        /// The data type for the event
        /// </summary>
        Type DataType { get; }
    }


    public interface EventCorrelation<TInstance, TData> :
        EventCorrelation
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        Event<TData> Event { get; }

        /// <summary>
        /// Returns the saga policy for the event correlation
        /// </summary>
        /// <value></value>
        ISagaPolicy<TInstance, TData> Policy { get; }

        /// <summary>
        /// The filter factory creates the filter when requested by the connector
        /// </summary>
        SagaFilterFactory<TInstance, TData> FilterFactory { get; }

        /// <summary>
        /// The message filter which extracts the correlationId from the message
        /// </summary>
        IFilter<ConsumeContext<TData>> MessageFilter { get; }
    }
}