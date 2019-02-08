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
namespace MassTransit.Topology
{
    using System.Reflection;


    public interface IMessageTopology<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The entity name formatter for this message type
        /// </summary>
        IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; }

        /// <summary>
        /// The formatted entity name for this message type
        /// </summary>
        string EntityName { get; }

        IMessagePropertyTopology<TMessage, T> GetProperty<T>(PropertyInfo propertyInfo);
    }


    public interface IMessageTopology :
        IMessageTopologyConfigurationObserverConnector
    {
        /// <summary>
        /// The entity name formatter used to format message names
        /// </summary>
        IEntityNameFormatter EntityNameFormatter { get; }

        /// <summary>
        /// Returns the message topology for the specified message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IMessageTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
