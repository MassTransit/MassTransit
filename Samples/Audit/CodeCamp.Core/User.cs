namespace CodeCamp.Core
{
	using Messages;

	public class User : IIdentifier
	{
		private readonly string _password;
		private readonly string _username;

		public User(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public string Username
		{
			get { return _username; }
		}

		#region IIdentifier Members

		public object Key
		{
			get { return _username; }
		}

		#endregion

		public bool CheckPassword(string password)
		{
			if (_password == password)
			{
				DomainContext.ServiceBus.Publish(new UserPasswordSuccess(_username));
				return true;
			}

			DomainContext.ServiceBus.Publish(new UserPasswordFailure(_username));

			return false;
		}
	}
}