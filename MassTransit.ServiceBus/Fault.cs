// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A fault is a system-generated message that is published when an exception occurs while processing a message.
	/// </summary>
	/// <typeparam name="TMessage">The type of message that threw the exception</typeparam>
	[Serializable]
	public class Fault<TMessage> where TMessage : class
	{
		private readonly TMessage _failedMessage;
		private readonly List<string> _messages;
		private readonly DateTime _occurredAt;
		private readonly List<string> _stackTrace;

		/// <summary>
		/// Creates a new fault message for the failed message
		/// </summary>
		/// <param name="ex">The exception thrown by the message consumer</param>
		/// <param name="message">The message that was being processed when the exception was thrown</param>
		public Fault(Exception ex, TMessage message)
		{
			_failedMessage = message;
			_occurredAt = DateTime.UtcNow;

			_messages = GetExceptionMessages(ex);
			_stackTrace = GetStackTrace(ex);
		}

		/// <summary>
		/// When the exception occurred
		/// </summary>
		public DateTime OccurredAt
		{
			get { return _occurredAt; }
		}

		/// <summary>
		/// Messages associated with the exception
		/// </summary>
		public IEnumerable<string> Messages
		{
			get { return _messages; }
		}

		/// <summary>
		/// A stack trace related to the exception
		/// </summary>
		public IEnumerable<string> StackTrace
		{
			get { return _stackTrace; }
		}

		/// <summary>
		/// The message that failed to be consumed
		/// </summary>
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

	/// <summary>
	/// A fault is a system-generated message that is published when an exception occurs while processing a message.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	[Serializable]
	public class Fault<TMessage, TKey> :
		Fault<TMessage>,
		CorrelatedBy<TKey>
		where TMessage : class, CorrelatedBy<TKey>
	{
		/// <summary>
		/// Creates a new Fault message for the failed correlated message
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="message"></param>
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