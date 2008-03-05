namespace MassTransit.ServiceBus.Formatters
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class XmlMessageFormatter :
		IMessageFormatter
	{
		private readonly XmlSerializer _serializer;
        private readonly Dictionary<Type, XmlSerializer> _genericSerializers;


		public XmlMessageFormatter()
		{
            MessageFinder.Initialize();
			List<Type> types = MessageFinder.AllNonGenericMessageTypes();

			_serializer = new XmlSerializer(typeof (object[]), types.ToArray());
            _genericSerializers = new Dictionary<Type, XmlSerializer>();
		}

		#region IMessageFormatter Members

		public void Serialize(IFormattedBody body, params IMessage[] messages)
		{
            Type type = messages[0].GetType();
            if (type.IsGenericType)
            {
                SerializeGeneric(body, messages);
            }
            else
            {
                List<object> objects = new List<object>();
                objects.AddRange(messages);

                _serializer.Serialize(body.BodyStream, objects.ToArray());
            }
		}

        private void SerializeGeneric(IFormattedBody body, params IMessage[] messages)
        {
            Type type = messages[0].GetType();
            if(!_genericSerializers.ContainsKey(type))
            {
                _genericSerializers.Add(type, new XmlSerializer(typeof (object[]), new Type[] {type}));
            }

            _genericSerializers[type].Serialize(body.BodyStream, messages);          
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
        private IMessage[] DeserializeGeneric(IFormattedBody formattedBody)
        {
            //?
            return null;
        }

		#endregion
	}
}