namespace WebRequestReply.Core
{
	using System;
	using MassTransit.ServiceBus;

	[Serializable]
	public class RequestMessage : CorrelatedBy<Guid>
	{
		private readonly Guid _id;
		private readonly string _text;

		public RequestMessage(Guid id, string text)
		{
			_text = text;
			_id = id;
		}

		public string Text
		{
			get { return _text; }
		}

		public Guid CorrelationId
		{
			get { return _id; }
		}
	}
}