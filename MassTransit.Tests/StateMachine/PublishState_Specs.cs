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
namespace MassTransit.Tests.StateMachine
{
	using System.Transactions;
	using NUnit.Framework;

	[TestFixture]
	public class PublishState_Specs
	{
		[Test]
		public void Model_the_state_of_publishing_a_message()
		{
			PublishState state = new PublishState();
		}
	}

	public class PublishState :
		StateMachineBase<PublishState>
	{
		static PublishState()
		{
			Define(() => Idle).AsInitial();
			Define(() => SendingInTransaction);
			Define(() => Sending);
			Define(() => AtLeastOneMessageSent);
			Define(() => Complete);


			Define(() => Called);
			Define(() => MessageSent);
			Define(() => LastMessageSent);

			Define(() => Called)
				.When(Idle, x =>
					{
						if (Transaction.Current != null)
							x.TransitionTo(SendingInTransaction);
						else
							x.TransitionTo(Sending);
					})
				.Otherwise(x =>
					{
						// perhaps we want to mask an error condition or something
						// the default should be to throw an invalid state change occurred exception
					});

			Define(() => MessageSent)
				.When(Sending, x => { x.TransitionTo(AtLeastOneMessageSent); });


			Define(() => SendExceptionOccurred)
				.When(SendingInTransaction, x =>
					{
						Transaction.Current.Rollback();
					});


			Idle
				.When(Called).TransitionTo(Sending);

			Sending
				.When(MessageSent).TransitionTo(AtLeastOneMessageSent)
				.When(LastMessageSent).TransitionTo(AllMessagesSent);

			AtLeastOneMessageSent
				.When(LastMessageSent).TransitionTo(AllMessagesSent);
		}

		public static StateEvent<PublishState> SendExceptionOccurred { get; set; }


		public static StateEvent<PublishState> LastMessageSent { get; set; }
		public static StateEvent<PublishState> MessageSent { get; set; }
		public static StateEvent<PublishState> Called { get; set; }

		public static State<PublishState> Idle { get; set; }
		public static State<PublishState> SendingInTransaction { get; set; }
		public static State<PublishState> Sending { get; set; }
		public static State<PublishState> AtLeastOneMessageSent { get; set; }
		public static State<PublishState> AllMessagesSent { get; set; }
		public static State<PublishState> Complete { get; set; }
	}
}