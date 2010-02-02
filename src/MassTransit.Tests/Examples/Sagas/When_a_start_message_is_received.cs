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
	public class When_a_start_message_is_received :
		Given_a_simple_saga_does_not_exist
	{
		[When]
		public void A_start_message_is_received()
		{
			Message = new StartSimpleSaga
				{
					CorrelationId = SagaId,
					CustomerId = CustomerId,
				};

			LocalBus.Publish(Message);
		}

		protected StartSimpleSaga Message { get; set; }
		protected const int CustomerId = 47;

		[Then]
		public void A_new_saga_should_be_created()
		{
			ExtensionMethodsForAssertions.ShouldNotBeNull(Saga);
		}

		[Then]
		public void The_customer_id_should_be_set()
		{
			ExtensionMethodsForAssertions.ShouldEqual(Saga.CustomerId, CustomerId);
		}
	}
}