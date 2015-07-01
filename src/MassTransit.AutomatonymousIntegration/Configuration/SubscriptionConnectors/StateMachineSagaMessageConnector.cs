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
namespace Automatonymous.SubscriptionConnectors
{
    using CorrelationConfigurators;
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.Connectors;


    public class StateMachineSagaMessageConnector<TInstance, TMessage> :
        SagaMessageConnector<TInstance, TMessage>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly IFilter<SagaConsumeContext<TInstance, TMessage>> _consumeFilter;
        readonly ISagaPolicy<TInstance, TMessage> _policy;
        readonly SagaFilterFactory<TInstance, TMessage> _sagaFilterFactory;
        IFilter<ConsumeContext<TMessage>> _messageFilter;

        public StateMachineSagaMessageConnector(IFilter<SagaConsumeContext<TInstance, TMessage>> consumeFilter, ISagaPolicy<TInstance, TMessage> policy,
            SagaFilterFactory<TInstance, TMessage> sagaFilterFactory, IFilter<ConsumeContext<TMessage>> messageFilter)
        {
            _consumeFilter = consumeFilter;
            _policy = policy;
            _sagaFilterFactory = sagaFilterFactory;
            _messageFilter = messageFilter;
        }

        protected override void ConfigureSagaPipe(IPipeConfigurator<SagaConsumeContext<TInstance, TMessage>> configurator)
        {
            configurator.UseFilter(_consumeFilter);
        }

        protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TInstance> repository,
            IPipe<SagaConsumeContext<TInstance, TMessage>> sagaPipe)
        {
            if (_messageFilter != null)
                configurator.UseFilter(_messageFilter);

            configurator.UseFilter(_sagaFilterFactory(repository, _policy, sagaPipe));
        }
    }
}