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
	using System.IO;
	using System.Runtime.Serialization;
	using Context;
	using Custom;

	public class VersionOneXmlMessageSerializer :
		IMessageSerializer
	{
		const string ContentTypeHeaderValue = "application/vnd.masstransit+xmlv1";

		static readonly IXmlSerializer _serializer = new CustomXmlSerializer();

		public string ContentType
		{
			get { return ContentTypeHeaderValue; }
		}

		public void Serialize<T>(Stream stream, ISendContext<T> context)
			where T : class
		{
			try
			{
				context.SetContentType(ContentTypeHeaderValue);

				XmlMessageEnvelope envelope = XmlMessageEnvelope.Create(context);

				_serializer.Serialize(stream, envelope, (declaringType, propertyType, value) =>
					{
						if (declaringType == typeof (XmlMessageEnvelope) && propertyType == typeof (object))
							return typeof (T);

						if (propertyType == typeof (object))
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

		public void Deserialize(IReceiveContext context)
		{
			try
			{
				object message = _serializer.Deserialize(context.BodyStream);

				if (message == null)
					throw new SerializationException("Could not deserialize message.");

				context.SetContentType(ContentTypeHeaderValue);

				if (message is XmlMessageEnvelope)
				{
					var envelope = message as XmlMessageEnvelope;

					context.SetUsingMessageEnvelope(envelope);
					context.SetMessageTypeConverter(new StaticMessageTypeConverter(envelope.Message));
					return;
				}

				context.SetMessageTypeConverter(new StaticMessageTypeConverter(message));
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