namespace CodeCamp.Messages
{
	using System;

	[Serializable]
	public class UserPasswordSuccess
	{
		private readonly DateTime _timeStamp;
		private readonly string _username;

		public UserPasswordSuccess(string username)
		{
			_timeStamp = DateTime.Now;
			_username = username;
		}

		public string Username
		{
			get { return _username; }
		}

		public DateTime TimeStamp
		{
			get { return _timeStamp; }
		}
	}
}