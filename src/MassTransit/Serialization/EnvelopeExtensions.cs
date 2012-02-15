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
	/// <summary>
	/// Extensions to the <see cref="Envelope"/> class dealing with transferring data
	/// from the context to the envelope or vice versa.
	/// </summary>
    public static class EnvelopeExtensions
    {
		/// <summary>
		/// Sets the contextual data based on what was found in the envelope. Used by the inbound
		/// transports as the receive context needs to be hydrated from the actual data that was 
		/// transferred through the transport as payload.
		/// </summary>
		/// <param name="context">The context to write data to, from the envelope</param>
		/// <param name="envelope">The envelope that contains the data to read into the context</param>
        public static void SetUsingEnvelope(this IReceiveContext context, Envelope envelope)
        {
            context.SetRequestId(envelope.RequestId);
            context.SetConversationId(envelope.ConversationId);
            context.SetCorrelationId(envelope.CorrelationId);
            context.SetSourceAddress(envelope.SourceAddress.ToUriOrNull());
            context.SetDestinationAddress(envelope.DestinationAddress.ToUriOrNull());
            context.SetResponseAddress(envelope.ResponseAddress.ToUriOrNull());
            context.SetFaultAddress(envelope.FaultAddress.ToUriOrNull());
            context.SetNetwork(envelope.Network);
            context.SetRetryCount(envelope.RetryCount);
            if (envelope.ExpirationTime.HasValue)
                context.SetExpirationTime(envelope.ExpirationTime.Value);

            foreach (var header in envelope.Headers)
            {
                context.SetHeader(header.Key, header.Value);
            }
        }

		/// <summary>
		/// Transfers all contextual data to the envelop. 
		/// As such it 'sets the envelope data to that of the context'. Used by the outbound
		/// transports as the envelope needs to be hydrated from the meta-data and message object
		/// that is being passed down the outbound pipeline to the transport.
		/// </summary>
		/// <param name="envelope">Envelope instance to hydrate with context data.</param>
		/// <param name="context">The context to take the contextual data from.</param>
        public static void SetUsingContext(this Envelope envelope, ISendContext context)
        {
            envelope.RequestId = context.RequestId;
            envelope.ConversationId = context.ConversationId;
            envelope.CorrelationId = context.CorrelationId;
            envelope.SourceAddress = context.SourceAddress.ToStringOrNull() ?? envelope.SourceAddress;
            envelope.DestinationAddress = context.DestinationAddress.ToStringOrNull() ?? envelope.DestinationAddress;
            envelope.ResponseAddress = context.ResponseAddress.ToStringOrNull() ?? envelope.ResponseAddress;
            envelope.FaultAddress = context.FaultAddress.ToStringOrNull() ?? envelope.FaultAddress;
            envelope.Network = context.Network;
            envelope.RetryCount = context.RetryCount;
            if (context.ExpirationTime.HasValue)
                envelope.ExpirationTime = context.ExpirationTime.Value;

            foreach (var header in context.Headers)
            {
                envelope.Headers[header.Key] = header.Value;
            }
        }
    }
}