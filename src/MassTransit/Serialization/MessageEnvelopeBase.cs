// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
    using System;

    /// <summary>
    ///   A base message envelope for transports that support enveloped messages
    ///   This does not include the binary formatter, since it is retained for
    ///   legacy support as a pure object formatter with no envelope
    /// </summary>
    public abstract class MessageEnvelopeBase
    {
        /// <summary>
        ///   The source where the message originated
        /// </summary>
        public string SourceAddress { get; set; }

        /// <summary>
        ///   The destination where the message was originally sent
        /// </summary>
        public string DestinationAddress { get; set; }

        /// <summary>
        ///   A transport specific message identifier if appropriate
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// The request identifier
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///   A higher-level conversation identifier that goes above any type of saga or request/response
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        ///   A correlation identifier for the message, if a saga or correlated message
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        ///   The destination to use for replies to this message
        /// </summary>
        public string ResponseAddress { get; set; }

        /// <summary>
        ///   The destination to send any faults caused by this message
        ///   If not specified, faults are either sent to the ResponseAddress address
        ///   If this nor ResponseAddress is specified, faults are published
        /// </summary>
        public string FaultAddress { get; set; }

        /// <summary>
        ///   Identifies a specific network to which this message belongs and is used to filter
        ///   out messages that might be from untrusted networks
        /// </summary>
        public string Network { get; set; }

        /// <summary>
        ///   The number of times the message has been retried by a consumer
        ///   Starts at zero and is incremented every time the message is scheduled for retry
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        ///   The message envelope base for the binary serializer?
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        ///   The type of the message, including the full name and assembly
        /// </summary>
        public string MessageType { get; set; }
    }
}