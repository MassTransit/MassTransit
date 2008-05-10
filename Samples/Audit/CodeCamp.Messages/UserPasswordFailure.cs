namespace CodeCamp.Messages
{
	using System;
	using MassTransit.ServiceBus;

	[Serializable]
	public class UserPasswordFailure : IMessage
	{
		private readonly DateTime _timeStamp;
		private readonly string _username;

		public UserPasswordFailure(string username)
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