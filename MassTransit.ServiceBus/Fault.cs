namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class Fault<TMessage> where TMessage : class
	{
		private readonly TMessage _failedMessage;
		private readonly List<string> _messages;
		private readonly DateTime _occurredAt;
		private readonly List<string> _stackTrace;

		public Fault(Exception ex, TMessage message)
		{
			_failedMessage = message;
			_occurredAt = DateTime.UtcNow;

			_messages = GetExceptionMessages(ex);
			_stackTrace = GetStackTrace(ex);
		}

		public DateTime OccurredAt
		{
			get { return _occurredAt; }
		}

		public IEnumerable<string> Messages
		{
			get { return _messages; }
		}

		public IEnumerable<string> StackTrace
		{
			get { return _stackTrace; }
		}

		public TMessage FailedMessage
		{
			get { return _failedMessage; }
		}

		private static List<string> GetStackTrace(Exception ex)
		{
			List<string> result = new List<string>();

			result.Add(string.IsNullOrEmpty(ex.StackTrace) ? "Stack Trace" : ex.StackTrace);

			Exception innerException = ex.InnerException;
			while (innerException != null)
			{
				string stackTrace = "InnerException Stack Trace: " + innerException.StackTrace;
				result.Add(stackTrace);

				innerException = innerException.InnerException;
			}

			return result;
		}

		private static List<string> GetExceptionMessages(Exception ex)
		{
			List<string> result = new List<string>();

			result.Add(ex.Message);

			Exception innerException = ex.InnerException;
			while (innerException != null)
			{
				result.Add(innerException.Message);

				innerException = innerException.InnerException;
			}

			return result;
		}
	}

	[Serializable]
	public class Fault<TMessage, TKey> :
		Fault<TMessage>,
		CorrelatedBy<TKey>
		where TMessage : class, CorrelatedBy<TKey>
	{
		public Fault(Exception ex, TMessage message) :
			base(ex, message)
		{
		}

		public TKey CorrelationId
		{
			get { return FailedMessage.CorrelationId; }
		}
	}
}