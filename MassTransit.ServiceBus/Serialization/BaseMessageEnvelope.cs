// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Serialization
{
    /// <summary>
    /// A base message envelope for transports that support enveloped messages
    /// This does not include the binary formatter, since it is retained for
    /// legacy support as a pure object formatter with no envelope
    /// </summary>
    public abstract class BaseMessageEnvelope
    {
        public string MessageType { get; set; }

        /// <summary>
        /// A transport specific message identifier if appropriate
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Indicates the message this message is related to, such as a response
        /// </summary>
        public string RelatedTo { get; set; }

        /// <summary>
        /// A transport-specific correlation identifier if appropriate
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The source where the message originated
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The destination where the message was originally sent
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// The destination to use for replies to this message
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// The destination to send any faults caused by this message
        /// If not specified, faults are either sent to the ReplyTo address
        /// If this nor ReplyTo is specified, faults are published
        /// </summary>
        public string FaultTo { get; set; }
    }
}