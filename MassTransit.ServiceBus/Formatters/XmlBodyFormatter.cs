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

		#region IMessageFormatter Members

		public void Serialize(IFormattedBody body, params IMessage[] messages)
		{
			List<object> objects = new List<object>();
			objects.AddRange(messages);

			_serializer.Serialize(body.BodyStream, objects.ToArray());
		}

		public IMessage[] Deserialize(IFormattedBody formattedBody)
		{
			object result = _serializer.Deserialize(formattedBody.BodyStream);

			List<IMessage> messages = new List<IMessage>();

			if (result is object[])
			{
				foreach (object o in (object[]) result)
				{
					messages.Add((IMessage) o);
				}
			}

			return messages.ToArray();
		}

		#endregion
	}
}