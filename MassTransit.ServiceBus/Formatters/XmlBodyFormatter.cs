namespace MassTransit.ServiceBus.Formatters
{
	using System.Xml.Serialization;
	using Util;

	public class XmlBodyFormatter :
		IBodyFormatter
	{
		private readonly XmlSerializer _serializer;

		public XmlBodyFormatter()
		{
			_serializer = new XmlSerializer(typeof (object));
		}

		public void Serialize(IFormattedBody body, object message)
		{
			Check.EnsureSerializable(message);
			_serializer.Serialize(body.BodyStream, message);
		}

		public T Deserialize<T>(IFormattedBody formattedBody) where T : class
		{
			object result = _serializer.Deserialize(formattedBody.BodyStream);


			if (typeof (T).IsAssignableFrom(result.GetType()))
				return (T) result;

			return default(T);
		}

		object IBodyFormatter.Deserialize(IFormattedBody formattedBody)
		{
			object result = _serializer.Deserialize(formattedBody.BodyStream);

			return result;
		}
	}
}