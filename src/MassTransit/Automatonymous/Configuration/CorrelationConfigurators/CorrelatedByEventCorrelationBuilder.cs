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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using MassTransit;


    public class CorrelatedByEventCorrelationBuilder<TInstance, TData> :
        IEventCorrelationBuilder
        where TData : class, CorrelatedBy<Guid>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly MassTransitEventCorrelationConfigurator<TInstance, TData> _configurator;

        public CorrelatedByEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<TData> @event)
        {
            var configurator = new MassTransitEventCorrelationConfigurator<TInstance, TData>(machine, @event, null);
            configurator.CorrelateById(x => x.Message.CorrelationId);

            _configurator = configurator;
        }

        public EventCorrelation Build()
        {
            return _configurator.Build();
        }
    }
}