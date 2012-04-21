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
namespace MassTransit.Distributor.WorkerConnectors
{
    using System;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Messages;
    using Pipeline;
    using Saga;

    public abstract class SagaWorkerConnectorBase<TSaga, TMessage> :
        SagaWorkerConnector
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _sagaRepository;

        public SagaWorkerConnectorBase(ISagaRepository<TSaga> sagaRepository)
        {
            _sagaRepository = sagaRepository;
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IWorker worker)
        {
            IWorkerLoad<TMessage> workerLoad = worker.GetWorkerLoad<TMessage>();

            ISagaPolicy<TSaga, TMessage> policy = GetPolicy();

            ISagaMessageSink<TSaga, TMessage> messageSink = GetSagaMessageSink(_sagaRepository, policy);

            var sink = new WorkerMessageSink<TMessage>(workerLoad, messageSink);

            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<Distributed<TMessage>>());
        }

        protected abstract ISagaPolicy<TSaga, TMessage> GetPolicy();

        protected abstract ISagaMessageSink<TSaga, TMessage> GetSagaMessageSink(ISagaRepository<TSaga> sagaRepository,
            ISagaPolicy<TSaga, TMessage> policy);
    }
}