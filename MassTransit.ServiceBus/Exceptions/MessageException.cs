namespace MassTransit.ServiceBus.Exceptions
{
	using System;

	[Serializable]
	public class MessageException :
		Exception
	{
		private readonly Type _messageType;

		public MessageException(Type messageType, string message, Exception innerException) :
			base(message, innerException)
		{
			_messageType = messageType;
		}

		public MessageException(Type messageType, string message) :
			base(message)
		{
			_messageType = messageType;
		}

		public MessageException()
		{
		}

		public Type MessageType
		{
			get { return _messageType; }
		}
	}
}