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
	using System.Collections.Generic;
	using System.Runtime.Remoting.Messaging;
	using Internal;
	using Magnum.ObjectExtensions;

	public class BinaryMessageEnvelope :
		MessageEnvelopeBase
	{
		private const string ConversationIdKey = "ConversationId";
		private const string CorrelationIdKey = "CorrelationId";
		private const string DestinationAddressKey = "DestinationAddress";
		private const string FaultAddressKey = "FaultAddress";
		private const string MessageIdKey = "MessageId";
		private const string MessageTypeKey = "MessageType";
		private const string ResponseAddressKey = "ResponseAddress";
		private const string RetryCountKey = "RetryCount";
		private const string SourceAddressKey = "SourceAddress";

		private BinaryMessageEnvelope()
		{
		}

		public Header[] ToHeaders()
		{
			List<Header> headers = new List<Header>();

			var context = OutboundMessage.Headers;

			if (context.SourceAddress != null)
			{
				headers.Add(new Header(SourceAddressKey, context.SourceAddress));
			}

			if (context.DestinationAddress != null)
			{
				headers.Add(new Header(DestinationAddressKey, context.DestinationAddress));
			}

			if (context.ResponseAddress != null)
			{
				headers.Add(new Header(ResponseAddressKey, context.ResponseAddress));
			}

			if (context.FaultAddress != null)
			{
				headers.Add(new Header(FaultAddressKey, context.FaultAddress));
			}

			if (!context.MessageType.IsNullOrEmpty())
			{
				headers.Add(new Header(MessageTypeKey, context.MessageType));
			}

			if (context.RetryCount > 0)
				headers.Add(new Header(RetryCountKey, context.RetryCount));

			return headers.ToArray();
		}

		private void MapNameValuePair(string name, object value)
		{
			switch (name)
			{
				case SourceAddressKey:
					SourceAddress = ((Uri) value).ToStringOrNull();
					break;

				case ResponseAddressKey:
					ResponseAddress = ((Uri) value).ToStringOrNull();
					break;

				case DestinationAddressKey:
					DestinationAddress = ((Uri) value).ToStringOrNull();
					break;

				case FaultAddressKey:
					FaultAddress = ((Uri) value).ToStringOrNull();
					break;

				case RetryCountKey:
					RetryCount = (int) value;
					break;

				case MessageTypeKey:
					MessageType = (string) value;
					break;
			}
		}

		public static BinaryMessageEnvelope From(IOutboundMessage context)
		{
			var envelope = new BinaryMessageEnvelope();

			envelope.CopyFrom(context);

			return envelope;
		}

		public static BinaryMessageEnvelope From(Header[] headers)
		{
			var envelope = new BinaryMessageEnvelope();

			if (headers != null)
			{
				for (int i = 0; i < headers.Length; i++)
				{
					envelope.MapNameValuePair(headers[i].Name, headers[i].Value);
				}
			}

			return envelope;
		}
	}
}