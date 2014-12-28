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
    using System;


    /// <summary>
    ///     API-oriented class.
    /// </summary>
    /// <typeparam name="TMessage">The message type to consume.</typeparam>
    public static class Consumes<TMessage>
        where TMessage : class
    {
        /// <summary>
        ///     Declares a Consume method for the message type TMessage which is called
        ///     whenever a a message is received of the specified type.
        /// </summary>
        [Obsolete("These interfaces are provided for legacy purposes only, use IConsumer<T> instead.")]
        public interface All :
            IMessageConsumer<TMessage>
        {
        }
    }
}