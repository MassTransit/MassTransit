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
    using System;
    using System.Linq.Expressions;


    public interface IMessageTopologyConfigurator<TMessage> :
        IMessageTypeTopologyConfigurator,
        IMessageTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Sets the entity name formatter used for this message type
        /// </summary>
        /// <param name="entityNameFormatter"></param>
        void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter);

        /// <summary>
        /// Sets the entity name for this message type
        /// </summary>
        /// <param name="entityName">The entity name</param>
        void SetEntityName(string entityName);

        /// <summary>
        /// Specify the property which should be used for the message CorrelationId
        /// </summary>
        /// <param name="propertyExpression"></param>
        void CorrelateBy(Expression<Func<TMessage, Guid>> propertyExpression);

        IMessagePropertyTopologyConfigurator<TMessage, T> GetProperty<T>(Expression<Func<TMessage, T>> propertyExpression);
    }


    public interface IMessageTopologyConfigurator :
        IMessageTopology
    {
        /// <summary>
        /// Replace the default entity name formatter
        /// </summary>
        void SetEntityNameFormatter(IEntityNameFormatter entityNameFormatter);

        new IMessageTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
