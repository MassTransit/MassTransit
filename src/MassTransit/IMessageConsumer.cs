// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    /// <summary>
    ///     Defines a class as a consumer of messages which implement T, either as a class or interface
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageConsumer<in TMessage> :
        IConsumer
        where TMessage : class
    {
        /// <summary>
        ///     Called by the framework when a message is available to be consumed. This
        ///     is called by a framework thread, so care should be used when accessing
        ///     any shared objects.
        /// </summary>
        /// <param name="message">The message to consume.</param>
        void Consume(TMessage message);
    }
}