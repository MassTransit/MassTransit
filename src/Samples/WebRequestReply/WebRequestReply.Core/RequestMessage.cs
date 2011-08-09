namespace WebRequestReply.Core
{
	using System;
	using MassTransit;

	public class RequestMessage :
		CorrelatedBy<Guid>
	{
		public RequestMessage(Guid id, string text)
		{
			CorrelationId = id;
			Text = text;
		}

		protected RequestMessage()
		{
		}

		public string Text { get; private set; }

		public Guid CorrelationId { get; private set; }
	}
}