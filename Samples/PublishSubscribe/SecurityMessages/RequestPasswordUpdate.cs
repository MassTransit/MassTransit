namespace SecurityMessages
{
    using System;

    [Serializable]
    public class RequestPasswordUpdate
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
