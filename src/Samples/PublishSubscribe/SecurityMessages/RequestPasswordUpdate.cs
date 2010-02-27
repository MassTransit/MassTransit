namespace SecurityMessages
{
	using System;
	using MassTransit;

	[Serializable]
	public class RequestPasswordUpdate :
		CorrelatedBy<Guid>
	{
		public RequestPasswordUpdate(string newPassword)
		{
			CorrelationId = Guid.NewGuid();
			NewPassword = newPassword;
		}

		protected RequestPasswordUpdate()
		{
		}

		public string NewPassword { get; set; }

		public Guid CorrelationId { get; set; }
	}
}