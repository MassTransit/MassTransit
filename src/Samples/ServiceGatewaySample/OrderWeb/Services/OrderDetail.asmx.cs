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
namespace OrderWeb.Services
{
	using System;
	using System.ComponentModel;
	using System.Threading;
	using System.Web;
	using System.Web.Services;
	using GatewayService.Interfaces;
	using Magnum;
	using MassTransit;
	using MassTransit.Actors;
	using MassTransit.Saga;
	using StructureMap;

	/// <summary>
	/// Summary description for OrderDetail
	/// </summary>
	[WebService(Namespace = "http://masstransit-project.com/Sample/OrderDetail")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
		// [System.Web.Script.Services.ScriptService]
	public class OrderDetail :
		WebService
	{
		public OrderDetail()
		{
			Bus = ObjectFactory.GetInstance<IServiceBus>();
			ActorRepository = ObjectFactory.GetInstance<IActorRepository<OrderDetailRequestActor>>();
		}

		public IActorRepository<OrderDetailRequestActor> ActorRepository { get; set; }
		public IServiceBus Bus { get; set; }

		[WebMethod]
		public string HelloWorld()
		{
			return "Hello World";
		}

		[WebMethod]
		public IAsyncResult BeginGetOrderDetails(string customerId, string orderId, AsyncCallback callback, object state)
		{
			Guid requestId = CombGuid.Generate();

			var actor = new OrderDetailRequestActor(requestId);

			ActorRepository.Add(actor);

			AsyncCallback asyncCallback = ar =>
			{
				ActorRepository.Remove(actor);

				callback(ar);
			};

			IAsyncResult asyncResult = actor.BeginAction(asyncCallback, state);

			Bus.Endpoint.Send(new InitiateOrderDetailsRequest()
				{
					RequestId = requestId,
					OrderId = orderId,
					CustomerId = customerId,
				});

			return asyncResult;
		}

		[WebMethod]
		public OrderDetails EndGetOrderDetails(IAsyncResult asyncResult)
		{
			var actor = asyncResult as OrderDetailRequestActor;
			if (actor == null)
				throw new InvalidOperationException("WTF");

			return new OrderDetails
				{
					Created = actor.OrderCreated.Value.ToUniversalTime().ToString("r"),
					Status = actor.OrderStatus.ToString(),
				};
		}
	}
}