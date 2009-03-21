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
	using System.IO;
	using System.Runtime.Remoting.Messaging;
	using System.Runtime.Serialization.Formatters.Binary;
	using Internal;
	using log4net;
	using Magnum.ObjectExtensions;
	using Util;

	/// <summary>
	/// The binary message serializer used the .NET BinaryFormatter to serialize
	/// message content. 
	/// </summary>
	public class BinaryMessageSerializer
		: IMessageSerializer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (BinaryMessageSerializer));

		private const string ConversationIdKey = "ConversationId";
		private const string CorrelationIdKey = "CorrelationId";
		private const string DestinationAddressKey = "DestinationAddress";
		private const string FaultAddressKey = "FaultAddress";
		private const string MessageIdKey = "MessageId";
		private const string MessageTypeKey = "MessageType";
		private const string ResponseAddressKey = "ResponseAddress";
		private const string RetryCountKey = "RetryCount";
		private const string SourceAddressKey = "SourceAddress";

		private static readonly BinaryFormatter _formatter = new BinaryFormatter();

		public void Serialize<T>(Stream output, T message)
		{
			Check.EnsureSerializable(message);

			_formatter.Serialize(output, message, GetHeaders());
		}

		public object Deserialize(Stream input)
		{
			object obj = _formatter.Deserialize(input, DeserializeHeaderHandler);

			return obj;
		}

		private static Header[] GetHeaders()
		{
			var context = OutboundMessage.Headers;

			List<Header> headers = new List<Header>();

			headers.Add(SourceAddressKey, context.SourceAddress);
			headers.Add(DestinationAddressKey, context.DestinationAddress);
			headers.Add(ResponseAddressKey, context.ResponseAddress);
			headers.Add(FaultAddressKey, context.FaultAddress);

			headers.Add(MessageTypeKey, context.MessageType);

			headers.Add(RetryCountKey, context.RetryCount);

			return headers.ToArray();
		}

		private static object DeserializeHeaderHandler(Header[] headers)
		{
			if (headers == null)
				return null;

			InboundMessageHeaders.SetCurrent(context =>
				{
					context.Reset();

					for (int i = 0; i < headers.Length; i++)
					{
						MapNameValuePair(context, headers[i]);
					}
				});

			return null;
		}

		private static void MapNameValuePair(ISetMessageHeaders context, Header header)
		{
			switch (header.Name)
			{
				case SourceAddressKey:
					context.SetSourceAddress(ConvertUriToString(header.Value));
					break;

				case ResponseAddressKey:
					context.SetResponseAddress(ConvertUriToString(header.Value));
					break;

				case DestinationAddressKey:
					context.SetDestinationAddress(ConvertUriToString(header.Value));
					break;

				case FaultAddressKey:
					context.SetFaultAddress(ConvertUriToString(header.Value));
					break;

				case RetryCountKey:
					context.SetRetryCount((int) header.Value);
					break;

				case MessageTypeKey:
					context.SetMessageType((string) header.Value);
					break;

				default:
					if(header.MustUnderstand)
					{
						_log.WarnFormat("The header was not understood: " + header.Name);
					}
					break;
			}
		}

		public static string ConvertUriToString(object value)
		{
			if(value == null)
				return string.Empty;

			if (value.GetType() != typeof(Uri))
				return string.Empty;

			return value.ToString();
		}
	}

	public static class ExtensionsForBinaryMessageSerializer
	{
		public static void Add(this List<Header> headers, string key, Uri uri)
		{
			if (uri == null)
				return;

			headers.Add(new Header(key, uri));
		}

		public static void Add(this List<Header> headers, string key, string value)
		{
			if (value.IsNullOrEmpty())
				return;

			headers.Add(new Header(key, value));
		}

		public static void Add(this List<Header> headers, string key, int value)
		{
			if (value == 0)
				return;

			headers.Add(new Header(key, value));
		}
	}
}