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
namespace MassTransit.Topology.Configuration
{
    public interface IPublishTopologyConfigurator :
        IPublishTopology
    {
        /// <summary>
        /// Adds a convention to the topology, which will be applied to every message type
        /// requested, to determine if a convention for the message type is available.
        /// </summary>
        /// <param name="convention">The Publish topology convention</param>
        void AddConvention(IPublishTopologyConvention convention);

        /// <summary>
        /// Add a Publish topology for a specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="topology">The topology</param>
        void AddMessagePublishTopology<T>(IMessagePublishTopology<T> topology)
            where T : class;
    }


    public interface IPublishTopologyConfigurator<T>
        where T : class
    {
        /// <summary>
        /// Sets the entity name to a static value (using the static entity name provider)
        /// </summary>
        string EntityName { set; }

        /// <summary>
        /// Sets the entity name provider to the specified class, which will be used to format
        /// the entity name
        /// </summary>
        /// <param name="formatter">The entity name provider</param>
        void SetEntityNameProvider(IEntityNameFormatter formatter);
    }
}