// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;


    /// <summary>
    /// Extension methods pertinent to service bus logic, but on
    /// type <see cref="Type"/> - handles different sorts of reflection
    /// logic.
    /// </summary>
    public static class ServiceBusExtensions
    {
        /// <summary>
        /// Transforms the type of message to a normalized string which can be used
        /// for naming a queue on a transport.
        /// </summary>
        /// <param name="messageType">The message class/interface type</param>
        /// <returns>The normalized name for this type</returns>
        public static string ToMessageName(this Type messageType)
        {
            return MessageUrn.ForType(messageType).ToString();
        }
    }
}