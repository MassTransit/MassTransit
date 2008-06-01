namespace WebRequestReply.Core
{
	using System;
	using MassTransit.ServiceBus;

	[Serializable]
	public class ResponseMessage : CorrelatedBy<Guid>
	{
		private readonly Guid _id;
		private readonly string _text;

		public ResponseMessage(Guid id, string text)
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