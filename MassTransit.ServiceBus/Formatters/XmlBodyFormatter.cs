namespace MassTransit.ServiceBus.Formatters
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class XmlBodyFormatter :
		IBodyFormatter
	{
		private readonly XmlSerializer _serializer;

		public XmlBodyFormatter()
		{
			List<Type> types = MessageFinder.AllMessageTypes();

			_serializer = new XmlSerializer(typeof (object[]), types.ToArray());
		}

		public void Serialize(IFormattedBody body, object message)
		{
			_serializer.Serialize(body.BodyStream, message);
		}

		public T Deserialize<T>(IFormattedBody formattedBody) where T : class
		{
			object result = _serializer.Deserialize(formattedBody.BodyStream);

			if (result.GetType() == typeof(T))
				return (T)result;

			return default(T);
		}

		object IBodyFormatter.Deserialize(IFormattedBody formattedBody)
		{
			object result = _serializer.Deserialize(formattedBody.BodyStream);

			return result;
		}
	}
}