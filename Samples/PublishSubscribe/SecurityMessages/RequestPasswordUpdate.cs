namespace SecurityMessages
{
    using System;
    using MassTransit;

	[Serializable]
    public class RequestPasswordUpdate :
        CorrelatedBy<Guid>
    {
        private readonly string _newPassword;
		private readonly Guid _correlationId;

    	public RequestPasswordUpdate(string newPassword)
        {
			_correlationId = Guid.NewGuid();
            _newPassword = newPassword;
        }

        public string NewPassword
        {
            get { return _newPassword; }
        }

		public Guid CorrelationId
		{
			get { return _correlationId; }
		}
    }
}
