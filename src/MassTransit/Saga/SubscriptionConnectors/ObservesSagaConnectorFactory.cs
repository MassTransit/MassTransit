// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga.SubscriptionConnectors
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using Pipeline.Filters;


    public class ObservesSagaConnectorFactory<TSaga, TMessage> :
        SagaConnectorFactory
        where TSaga : class, ISaga, Observes<TMessage, TSaga>
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly ObservesSagaMessageFilter<TSaga, TMessage> _consumeFilter;
        readonly Expression<Func<TSaga, TMessage, bool>> _filterExpression;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public ObservesSagaConnectorFactory()
        {
            _policy = new ExistingOrIgnoreSagaPolicy<TSaga, TMessage>(x => false);

            var instance = (TSaga)FormatterServices.GetUninitializedObject(typeof(TSaga));
            _filterExpression = instance.GetBindExpression();

            _consumeFilter = new ObservesSagaMessageFilter<TSaga, TMessage>();
        }

        public SagaMessageConnector CreateMessageConnector()
        {
            return new PropertySagaMessageConnector<TSaga, TMessage>(_consumeFilter, _policy, _filterExpression);
        }
    }
}