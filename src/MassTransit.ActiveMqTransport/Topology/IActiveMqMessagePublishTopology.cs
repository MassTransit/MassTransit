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
namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using Builders;
    using Entities;
    using MassTransit.Topology;


    public interface IActiveMqMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IActiveMqMessagePublishTopology
        where TMessage : class
    {
        Topic Topic { get; }

        /// <summary>
        /// Returns the send settings for a publish endpoint, which are mostly unused now with topology
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        SendSettings GetSendSettings(Uri hostAddress);

        BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options = PublishBrokerTopologyOptions.MaintainHierarchy);
    }


    public interface IActiveMqMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}