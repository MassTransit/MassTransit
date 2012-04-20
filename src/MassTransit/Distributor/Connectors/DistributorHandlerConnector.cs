// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.Connectors
{
    using System;
    using Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Pipeline;
    using Subscriptions;

    public class DistributorHandlerConnector<TMessage> :
        DistributorConnector
        where TMessage : class
    {
        readonly IWorkerSelector<TMessage> _workerSelector;
        readonly Func<UnsubscribeAction, ISubscriptionReference> _referenceFactory;

        public DistributorHandlerConnector(IWorkerSelector<TMessage> workerSelector,
            Func<UnsubscribeAction, ISubscriptionReference> referenceFactory)
        {
            _workerSelector = workerSelector;
            _referenceFactory = referenceFactory;
        }

        public ISubscriptionReference Connect(IInboundPipelineConfigurator configurator, IDistributor distributor)
        {
            IWorkerAvailability<TMessage> workerAvailability = distributor.GetWorkerAvailability<TMessage>();

            var sink = new DistributorMessageSink<TMessage>(workerAvailability, _workerSelector);

            UnsubscribeAction unsubscribeAction = configurator.Pipeline.ConnectToRouter(sink,
                () => configurator.SubscribedTo<TMessage>());

            return _referenceFactory(unsubscribeAction);
        }
    }
}