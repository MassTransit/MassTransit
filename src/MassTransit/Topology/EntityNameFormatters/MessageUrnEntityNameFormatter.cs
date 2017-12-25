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
namespace MassTransit.Topology.EntityNameFormatters
{
    /// <summary>
    /// This is the simplest thing, it uses the built-in URN for a message type
    /// as the entity name, which can include illegal characters for most message
    /// brokers. It's nice for in-memory though, which doesn't give a hoot about the
    /// string.
    /// </summary>
    public class MessageUrnEntityNameFormatter :
        IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return new MessageUrn(typeof(T)).ToString();
        }
    }
}