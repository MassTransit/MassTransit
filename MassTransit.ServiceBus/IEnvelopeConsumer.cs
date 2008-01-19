/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// Implemented by consumers of messages
    /// </summary>
    public interface IEnvelopeConsumer
    {
        /// <summary>
        /// Called when a message is available from the endpoint. If the consumer returns true, the message
        /// will be removed from the endpoint and delivered to the consumer
        /// </summary>
        /// <param name="envelope">The message envelope available</param>
        /// <returns>True is the consumer will handle the message, false if it should be ignored</returns>
        bool IsHandled(IEnvelope envelope);

        /// <summary>
        /// Delivers the message envelope to the consumer
        /// </summary>
        /// <param name="envelope">The message envelope</param>
        void Deliver(IEnvelope envelope);
    }
}