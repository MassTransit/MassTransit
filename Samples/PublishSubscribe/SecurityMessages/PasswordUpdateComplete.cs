namespace SecurityMessages
{
	using System;
	using MassTransit.ServiceBus;

	[Serializable]
	public class PasswordUpdateComplete : CorrelatedBy<Guid>
	{
		private readonly Guid _correlationId;
		private readonly int _errorCode;

		public PasswordUpdateComplete(Guid correlationId, int errorCode)
		{
			_correlationId = correlationId;
			_errorCode = errorCode;
		}

		public int ErrorCode
		{
			get { return _errorCode; }
		}

		public Guid CorrelationId
		{
			get { return _correlationId; }
		}
	}
}