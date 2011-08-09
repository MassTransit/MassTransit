namespace WebRequestReply.Core
{
	using System;
	using MassTransit;

	public class ResponseMessage :
		CorrelatedBy<Guid>
	{
		public ResponseMessage(Guid id, string text)
		{
			CorrelationId = id;
			Text = text;
		}

		protected ResponseMessage()
		{
		}

		public string Text { get; private set; }
		public Guid CorrelationId { get; private set; }
	}
}