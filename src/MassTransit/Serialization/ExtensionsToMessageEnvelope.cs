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
namespace MassTransit.Serialization
{
	using System;
	using MessageHeaders;

	public static class ExtensionsToMessageEnvelope
	{
		public static Action<ISetInboundMessageHeaders> GetMessageHeadersSetAction(this MessageEnvelopeBase envelope)
		{
			return headers =>
				{
					headers.Reset();
					headers.SetSourceAddress(envelope.SourceAddress);
					headers.SetDestinationAddress(envelope.DestinationAddress);
					headers.SetResponseAddress(envelope.ResponseAddress);
					headers.SetFaultAddress(envelope.FaultAddress);
					headers.SetNetwork(envelope.Network);
					headers.SetRetryCount(envelope.RetryCount);
					headers.SetMessageType(envelope.MessageType);
					if(envelope.ExpirationTime.HasValue)
						headers.SetExpirationTime(envelope.ExpirationTime.Value);
				};
		}

		public static void CopyFrom(this MessageEnvelopeBase envelope, IMessageHeaders headers)
		{
			envelope.SourceAddress = headers.SourceAddress.ToStringOrNull() ?? envelope.SourceAddress;
			envelope.DestinationAddress = headers.DestinationAddress.ToStringOrNull() ?? envelope.DestinationAddress;
			envelope.ResponseAddress = headers.ResponseAddress.ToStringOrNull() ?? envelope.ResponseAddress;
			envelope.FaultAddress = headers.FaultAddress.ToStringOrNull() ?? envelope.FaultAddress;
			envelope.Network = headers.Network;
			envelope.RetryCount = headers.RetryCount;
			envelope.MessageType = headers.MessageType ?? envelope.MessageType;
			if(headers.ExpirationTime.HasValue)
				envelope.ExpirationTime = headers.ExpirationTime.Value;
		}

		public static string ToStringOrNull(this Uri uri)
		{
			return uri == null ? null : uri.ToString();
		}
	}
}