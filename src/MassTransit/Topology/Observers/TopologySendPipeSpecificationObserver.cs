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
namespace MassTransit.Topology.Observers
{
    using SendPipeSpecifications;
    using Specifications;


    public class TopologySendPipeSpecificationObserver :
        ISendPipeSpecificationObserver
    {
        readonly ISendTopology _topology;

        public TopologySendPipeSpecificationObserver(ISendTopology topology)
        {
            _topology = topology;
        }

        void ISendPipeSpecificationObserver.MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
        {
            IMessageSendTopology<T> messageSendTopology = _topology.GetMessageTopology<T>();

            var topologySpecification = new MessageSendTopologyPipeSpecification<T>(messageSendTopology);

            specification.AddParentMessageSpecification(topologySpecification);
        }
    }
}