namespace SecurityMessages
{
    using System;
    using MassTransit.ServiceBus;

    [Serializable]
    public class RequestPasswordUpdate : IMessage
    {
        private string _newPassword;

		private RequestPasswordUpdate()
		{
		}

    	public RequestPasswordUpdate(string newPassword)
        {
            _newPassword = newPassword;
        }


        public string NewPassword
        {
            get { return _newPassword; }
            set { _newPassword = value; }
        }
    }
}
