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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;


    public interface ITypeMetadataCache<out T>
    {
        string ShortName { get; }

        /// <summary>
        /// True if the type implements any known saga interfaces
        /// </summary>
        bool HasSagaInterfaces { get; }

        /// <summary>
        /// True if the message type is a valid message type
        /// </summary>
        bool IsValidMessageType { get; }
        
        /// <summary>
        /// Once checked, the reason why the message type is invalid
        /// </summary>
        string InvalidMessageTypeReason { get; }

        /// <summary>
        /// True if this message is not a public type
        /// </summary>
        bool IsTemporaryMessageType { get; }

        /// <summary>
        /// Returns all valid message types that are contained within the s
        /// </summary>
        Type[] MessageTypes { get; }

        /// <summary>
        /// The names of all the message types supported by the message type
        /// </summary>
        string[] MessageTypeNames { get; }

        IEnumerable<PropertyInfo> Properties { get; }

        T InitializeFromObject(object values);
    }
}