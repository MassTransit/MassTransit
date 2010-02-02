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
namespace MassTransit.Tests.Examples.Sagas
{
	using Messages;
	using TestFramework;

	[Scenario]
	public class When_an_approval_message_is_published :
		Given_a_simple_saga_exists_and_is_waiting_for_approval
	{
		[When]
		public void A_dynamically_correlated_message_is_published()
		{
			Message = new ApproveSimpleCustomer()
				{
					CustomerId = CustomerId,
				};

			LocalBus.Publish(Message);
		}

		protected ApproveSimpleCustomer Message { get; set; }

		[Then]
		public void The_saga_should_be_in_the_waiting_for_finish_state()
		{
			Saga.ShouldBeInState(SimpleSaga.WaitingForFinish);
		}
	}
}