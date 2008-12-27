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
namespace MassTransit.Tests.Saga.StateMachine
{
	using System;
	using System.Runtime.Serialization;
	using Magnum.Common.StateMachine;
	using Messages;

	[Serializable]
	public class RegisterUserStateMachine :
		SagaStateMachine<RegisterUserStateMachine>
	{
		static RegisterUserStateMachine()
		{
			Define(() =>
				{
					Initially(
						When(RegisterUserReceived, (w, e) =>
							{
								// e.Message would include the message
								w.TransitionTo(Completed);
							}));
				});
		}

		public RegisterUserStateMachine()
		{}

		public RegisterUserStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}

		public static MessageEvent<RegisterUser> RegisterUserReceived { get; set; }

		public static State Initial { get; set; }
		public static State Completed { get; set; }

		public void Consumes(RegisterUser message)
		{
			RaiseEvent(RegisterUserReceived, message);
		}
	}

	public class MessageEvent<T, M> :
		Event<T>,
		MessageEvent<M>
		where T : StateMachine<T>
		where M : class
	{
		public MessageEvent(string name)
			: base(name)
		{}
	}

	public interface MessageEvent<M> :
		Event
		where M : class
	{}

	public class SagaStateMachine<T> :
		StateMachine<T>
		where T : SagaStateMachine<T>
	{
		protected SagaStateMachine()
		{}

		public SagaStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}
	}
}