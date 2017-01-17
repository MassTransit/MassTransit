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
namespace MassTransit.EndpointSpecifications
{
    using ConsumePipeSpecifications;
    using GreenPipes;
    using Pipeline;
    using PublishPipeSpecifications;
    using SendPipeSpecifications;
    using Topology.Configuration;


    public interface IEndpointConfiguration :
        ISpecification
    {
        IConsumePipeSpecification ConsumePipeSpecification { get; }
        ISendPipeSpecification SendPipeSpecification { get; }
        IPublishPipeSpecification PublishPipeSpecification { get; }

        IConsumePipeConfigurator ConsumePipeConfigurator { get; }
        ISendPipeConfigurator SendPipeConfigurator { get; }
        IPublishPipeConfigurator PublishPipeConfigurator { get; }

        IConsumePipe CreateConsumePipe();
        ISendPipe CreateSendPipe();
        IPublishPipe CreatePublishPipe();

        void SeparatePublishFromSendTopology();
    }


    public interface IEndpointConfiguration<out TConfiguration, out TConsumeTopology, out TSendTopology, out TPublishTopology> :
        IEndpointConfiguration
        where TConfiguration : IEndpointConfiguration<TConfiguration, TConsumeTopology, TSendTopology, TPublishTopology>
        where TConsumeTopology : IConsumeTopologyConfigurator
        where TPublishTopology : IPublishTopologyConfigurator
        where TSendTopology : ISendTopologyConfigurator
    {
        TConsumeTopology ConsumeTopology { get; }
        TSendTopology SendTopology { get; }
        TPublishTopology PublishTopology { get; }

        /// <summary>
        /// Create a specification based upon this specification. All of the specifications and topologies will
        /// include what's already configured, EXCEPT for the Consume topology, which is unique per endpoint.
        /// </summary>
        /// <param name="consumePipe"></param>
        /// <returns></returns>
        TConfiguration CreateConfiguration(IConsumePipe consumePipe = null);
    }
}