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
	using System.IO;
	using System.Runtime.Serialization;
	using Custom;
	using Internal;

	public class XmlMessageSerializer :
		IMessageSerializer
	{
		private static readonly IXmlSerializer _serializer = new CustomXmlSerializer();

		public void Serialize<T>(Stream stream, T message)
		{
			try
			{
				var envelope = XmlMessageEnvelope.Create(message);

				_serializer.Serialize(stream, envelope, (declaringType, propertyType, value) =>
					{
						if (declaringType == typeof(XmlMessageEnvelope) && propertyType == typeof(object))
							return typeof (T);

						if (propertyType == typeof(object))
							return value.GetType();

						return propertyType;
					});
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new SerializationException("Failed to serialize message", ex);
			}
		}

		public object Deserialize(Stream stream)
		{
			try
			{
				object message = _serializer.Deserialize(stream);

				if (message == null)
					throw new SerializationException("Could not deserialize message.");

				if (message is XmlMessageEnvelope)
				{
					var envelope = message as XmlMessageEnvelope;

					InboundMessageHeaders.SetCurrent(envelope.GetMessageHeadersSetAction());

					return envelope.Message;
				}

				return message;
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new SerializationException("Failed to serialize message", ex);
			}
		}
	}
}