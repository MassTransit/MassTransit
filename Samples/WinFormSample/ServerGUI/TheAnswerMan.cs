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
namespace ServerGUI
{
	using System.Threading;
	using MassTransit;
	using Messages;

	public class TheAnswerMan :
		Consumes<SubmitQuestion>.Selected
	{
		private static bool _enabled = true;
		private static int _messageCount;
		private static volatile int _serverTime;
		private static int _messagesSent;

		public static int MessagesSent
		{
			get { return _messagesSent; }
		}

		public static bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}


		public IServiceBus Bus { get; set; }

		public static int MessageCount
		{
			get { return _messageCount; }
		}

		public static int ServerTime
		{
			get { return _serverTime; }
			set { _serverTime = value; }
		}

		public void Consume(SubmitQuestion message)
		{
			Thread.Sleep(_serverTime);

			Interlocked.Increment(ref _messageCount);

			QuestionAnswered answer = new QuestionAnswered(message.CorrelationId);

			CurrentMessage.Respond(answer);

			//Bus.Publish(answer);

			Interlocked.Increment(ref _messagesSent);
		}

		public bool Accept(SubmitQuestion message)
		{
			return _enabled;
		}
	}
}