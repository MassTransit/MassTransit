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
namespace GatewayService.Tests
{
	using Interfaces;
	using Magnum.DateTimeExtensions;
	using MassTransit;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class Saga_Specs :
		LoopbackTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			ObjectBuilder
				.Stub(x => x.GetInstance<OrderDetailsWebServiceProxy>())
				.Return(new OrderDetailsWebServiceProxy());

			SetupSagaRepository<OrderDetailsRequestSaga>(ObjectBuilder);

			LocalBus.Subscribe<OrderDetailsWebServiceProxy>();
			LocalBus.Subscribe<OrderDetailsRequestSaga>();
		}

		[Test]
		public void Should_work_through_the_saga()
		{
			var response = new FutureMessage<OrderDetailsReceived>();

			const string orderId = "ABC123";
			const string customerId = "12345";

			LocalBus.Subscribe<OrderDetailsReceived>(message =>
				{
					response.Set(message);
				},
				x => x.OrderId == orderId && x.CustomerId == customerId);

			RetrieveOrderDetails request = new RetrieveOrderDetailsRequest(customerId, orderId);
			LocalBus.Publish(request, x => x.SendResponseTo(LocalBus.Endpoint));

			Assert.IsTrue(response.WaitUntilAvailable(555.Seconds()), "The response was not received");

			Assert.AreEqual(orderId, response.Message.OrderId);
			Assert.AreEqual(customerId, response.Message.CustomerId);
			
		}
	}
}