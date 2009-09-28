namespace SecurityMessages
{
	using System;
	using MassTransit;

	[Serializable]
	public class PasswordUpdateComplete :
		CorrelatedBy<Guid>
	{
		public PasswordUpdateComplete(Guid correlationId, int errorCode)
		{
			CorrelationId = correlationId;
			ErrorCode = errorCode;
		}

		protected PasswordUpdateComplete()
		{
		}

		public int ErrorCode { get; set; }

		public Guid CorrelationId { get; set; }
	}
}