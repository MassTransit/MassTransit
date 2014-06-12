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
namespace MassTransit.Distributor.DistributorConnectors
{
    using System;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Pipeline;

    public interface MessageDistributorConnector
    {
        Type MessageType { get; }

        ConnectHandle Connect(IInboundMessagePipe configurator, IDistributor distributor);
    }

    public class MessageDistributorConnector<TMessage> :
        MessageDistributorConnector
        where TMessage : class
    {
        readonly IWorkerSelectorFactory _workerSelectorFactory;

        public MessageDistributorConnector(IWorkerSelectorFactory workerSelectorFactory)
        {
            _workerSelectorFactory = workerSelectorFactory;
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public ConnectHandle Connect(IInboundMessagePipe configurator, IDistributor distributor)
        {
            IWorkerAvailability<TMessage> workerAvailability = distributor.GetWorkerAvailability<TMessage>();

            IWorkerSelector<TMessage> workerSelector = _workerSelectorFactory.GetSelector<TMessage>();

            throw new NotImplementedException();
    
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IDistributor distributor)
        {
            IWorkerAvailability<TMessage> workerAvailability = distributor.GetWorkerAvailability<TMessage>();

            IWorkerSelector<TMessage> workerSelector = _workerSelectorFactory.GetSelector<TMessage>();

            var sink = new DistributorMessageSink<TMessage>(workerAvailability, workerSelector);
            throw new NotImplementedException();

//            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
        }
    }
}