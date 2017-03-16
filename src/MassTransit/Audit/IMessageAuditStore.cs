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
namespace MassTransit.Audit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Used to store message audits that are observed
    /// </summary>
    public interface IMessageAuditStore
    {
        /// <summary>
        /// Store the message audit, with associated metadata
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message itself</param>
        /// <param name="metadata">The message metadata</param>
        /// <returns></returns>
        Task StoreMessage<T>(T message, MessageAuditMetadata metadata)
            where T : class;
    }
}