namespace MassTransit.Serialization
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Custom;
	using Internal;
	using log4net;

	public class XmlMessageSerializer :
		IMessageSerializer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (XmlMessageSerializer));
		private static readonly IXmlSerializer _serializer = new CustomXmlSerializer();

		public void Serialize<T>(Stream stream, T message)
		{
			try
			{
				var envelope = XmlMessageEnvelope.Create(message);

				_serializer.Serialize(stream, envelope);
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new SerializationException("Failed to serialize message", ex);
			}
//			if(_log.IsDebugEnabled)
//				_log.Debug(Encoding.UTF8.GetString(_serializer.Serialize(envelope)));
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
					XmlMessageEnvelope envelope = message as XmlMessageEnvelope;

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