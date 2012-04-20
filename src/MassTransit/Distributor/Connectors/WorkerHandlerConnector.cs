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
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Messages;
    using Pipeline;
    using Subscriptions;

    public class WorkerHandlerConnector<TMessage> :
        WorkerConnector
        where TMessage : class
    {
        readonly HandlerSelector<TMessage> _handler;
        readonly ReferenceFactory _referenceFactory;

        public WorkerHandlerConnector(HandlerSelector<TMessage> handler,
            ReferenceFactory referenceFactory)
        {
            _handler = handler;
            _referenceFactory = referenceFactory;
        }

        public ISubscriptionReference Connect(IInboundPipelineConfigurator configurator, IWorker worker)
        {
            IWorkerLoad<TMessage> workerLoad = worker.GetWorkerLoad<TMessage>();

            var sink = new WorkerMessageSink<TMessage>(workerLoad, MultipleHandlerSelector.ForHandler(_handler));

            UnsubscribeAction unsubscribeAction = configurator.Pipeline.ConnectToRouter(sink,
                () => configurator.SubscribedTo<Distributed<TMessage>>());

            return _referenceFactory(unsubscribeAction);
        }
    }
}