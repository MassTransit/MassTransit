// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EndpointSpecifications
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using Pipeline;


    public class EndpointConfiguration :
        IEndpointConfiguration
    {
        protected EndpointConfiguration(ITopologyConfiguration topology, IConsumePipe consumePipe = null)
        {
            Topology = topology;

            Consume = new ConsumePipeConfiguration(consumePipe);
            Send = new SendPipeConfiguration(topology.Send);
            Publish = new PublishPipeConfiguration(topology.Publish);

            Serialization = new SerializationConfiguration();
        }

        protected EndpointConfiguration(IEndpointConfiguration parentConfiguration, ITopologyConfiguration topology, IConsumePipe consumePipe = null)
        {
            Topology = topology;

            Consume = new ConsumePipeConfiguration(parentConfiguration.Consume.Specification, consumePipe);
            Send = new SendPipeConfiguration(parentConfiguration.Send.Specification);
            Publish = new PublishPipeConfiguration(parentConfiguration.Publish.Specification);

            Serialization = parentConfiguration.Serialization.CreateSerializationConfiguration();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Send.Specification.Validate()
                .Concat(Publish.Specification.Validate())
                .Concat(Consume.Specification.Validate())
                .Concat(Topology.Send.Validate())
                .Concat(Topology.Publish.Validate())
                .Concat(Topology.Consume.Validate());
        }

        public IConsumePipeConfiguration Consume { get; }
        public ISendPipeConfiguration Send { get; }
        public IPublishPipeConfiguration Publish { get; }
        public ITopologyConfiguration Topology { get; }
        public ISerializationConfiguration Serialization { get; }
    }
}