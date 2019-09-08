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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public class UncorrelatedEventCorrelation<TInstance, TData> :
        EventCorrelation<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        public UncorrelatedEventCorrelation(Event<TData> @event)
        {
            Event = @event;
        }

        public SagaFilterFactory<TInstance, TData> FilterFactory { get; } = null;

        public Event<TData> Event { get; }

        Type EventCorrelation.DataType => typeof(TData);

        IFilter<ConsumeContext<TData>> EventCorrelation<TInstance, TData>.MessageFilter { get; } = null;

        ISagaPolicy<TInstance, TData> EventCorrelation<TInstance, TData>.Policy { get; } = null;

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure(Event.Name, "Correlation", "was not specified");
        }
    }
}