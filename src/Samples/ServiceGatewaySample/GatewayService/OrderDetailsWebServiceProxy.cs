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
namespace GatewayService
{
	using Interfaces;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit;
	using Messages;

	public class OrderDetailsWebServiceProxy :
		Consumes<RetrieveOrderDetails>.All
	{
		public void Consume(RetrieveOrderDetails request)
		{
			// simulate a call to the external service

			ThreadUtil.Sleep(1.Seconds());

			var details = new OrderDetailsResponse
				{
					OrderId = request.OrderId,
					CustomerId = request.CustomerId,
					Created = (-1).Days().FromUtcNow(),
					Status = OrderStatus.InProcess,
				};

			CurrentMessage.Respond(details, x => x.ExpiresAt(5.Minutes().FromNow()));
		}
	}
}