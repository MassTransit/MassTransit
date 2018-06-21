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
namespace MassTransit
{
    using System;


    /// <summary>
    /// Configures a message filter, for including and excluding message types
    /// </summary>
    public interface IMessageFilterConfigurator
    {
        /// <summary>
        /// Include the message if it is any of the specified message types
        /// </summary>
        /// <param name="messageTypes"></param>
        void Include(params Type[] messageTypes);

        /// <summary>
        /// Include the message if it is the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        void Include<T>() where T : class;

        /// <summary>
        /// Include the message if it is the specified message type and matches the specified filter expression
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">The filter expression</param>
        void Include<T>(Func<T, bool> filter) where T : class;

        /// <summary>
        /// Exclude the message if it is any of the specified message types
        /// </summary>
        /// <param name="messageTypes"></param>
        void Exclude(params Type[] messageTypes);

        /// <summary>
        /// Exclude the message if it is the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        void Exclude<T>() where T : class;

        /// <summary>
        /// Exclude the message if it is the specified message type and matches the specified filter expression
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">The filter expression</param>
        void Exclude<T>(Func<T, bool> filter) where T : class;
    }
}