namespace MassTransit.ServiceBus
{
	using System;

	/// <summary>
	/// Specifies the elapsed time before a message expires. When a message expires, the content is no longer
	/// // important and it can be automatically discarded by the message service.
	/// </summary>
	public class ExpiresInAttribute : Attribute
	{
		private readonly TimeSpan _timeSpan;

		/// <summary>
		/// Specifies the elapsed time before the message expires.
		/// </summary>
		/// <param name="timeSpanValue">The duration of the time period.</param>
		public ExpiresInAttribute(string timeSpanValue)
		{
			TimeSpan value;
			if (!TimeSpan.TryParse(timeSpanValue, out value))
				throw new ArgumentException("Unable to convert string to TimeSpan", "timeSpanValue");

			_timeSpan = value;
		}

		/// <summary>
		/// Returns the TimeSpan for the message expiration
		/// </summary>
		public TimeSpan TimeSpan
		{
			get { return _timeSpan; }
		}
	}
}