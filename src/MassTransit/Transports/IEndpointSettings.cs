// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using Serialization;

    public interface IEndpointSettings :
        ITransportSettings
    {
        /// <summary>
        /// The address of the endpoint where invalid messages should be moved
        /// </summary>
        IEndpointAddress ErrorAddress { get; }

        /// <summary>
        /// The serializer to use for messages on the endpoint
        /// </summary>
        IMessageSerializer Serializer { get; }

        /// <summary>
        /// The message serializers supported
        /// </summary>
        ISupportedMessageSerializers SupportedSerializers { get; }

        /// <summary>
        /// The retry limit for inbound messages
        /// </summary>
        int RetryLimit { get; }

        /// <summary>
        /// The message tracker factory
        /// </summary>
        MessageTrackerFactory TrackerFactory { get; }
    }
}