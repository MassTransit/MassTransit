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
    public interface IBusTopology
    {
        IPublishTopology PublishTopology { get; }
        
        ISendTopology SendTopology { get; }

        /// <summary>
        /// Returns the publish topology for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessagePublishTopology<T> Publish<T>()
            where T : class;

        /// <summary>
        /// Returns the send topology for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageSendTopology<T> Send<T>()
            where T : class;

        /// <summary>
        /// Returns the message topology for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageTopology<T> Message<T>()
            where T : class;
    }
}