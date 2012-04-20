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
    using MassTransit.Pipeline.Sinks;
    using Messages;
    using Pipeline;

    public class SelectedConsumerWorkerConnector<TConsumer, TMessage> :
        ConsumerWorkerConnector
        where TMessage : class
        where TConsumer : class, Consumes<TMessage>.Selected
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public SelectedConsumerWorkerConnector(IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IWorker worker)
        {
            IWorkerLoad<TMessage> workerLoad = worker.GetWorkerLoad<TMessage>();

            var consumerSink = new SelectedConsumerMessageSink<TConsumer, TMessage>(_consumerFactory);

            var sink = new WorkerMessageSink<TMessage>(workerLoad, consumerSink);

            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<Distributed<TMessage>>());
        }
    }
}