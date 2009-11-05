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
namespace OrderWeb
{
	using System;
	using GatewayService.Interfaces;
	using Magnum;
	using MassTransit;
	using MassTransit.Actors;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Saga;
	using Services;
	using StructureMap;

	public static class Bootstrapper
	{
		public static void Bootstrap()
		{
			BootstrapContainer();

			ActorBootstrapper.Bootstrap();
		}

		private static void BootstrapContainer()
		{
			var repository = new InMemoryActorRepository<OrderDetailRequestActor>();

			ObjectFactory.Configure(x =>
				{
					x.AddRegistry(new OrderWebRegistry());

					x.For<IActorRepository<OrderDetailRequestActor>>()
						.Use(repository);

					x.For<ISagaRepository<OrderDetailRequestActor>>()
						.Use(repository);
				});
		}
	}


	public static class ActorBootstrapper
	{
		public static void Bootstrap()
		{
			var bus = ObjectFactory.GetInstance<IServiceBus>();

			bus.Subscribe<OrderDetailRequestActor>();

			bus.Subscribe<RetrieveOrderDetails>(message =>
				{
					var details = new OrderDetailsReceivedImpl(message.CustomerId, message.OrderId, SystemUtil.Now, OrderStatus.Accepted);

					// TODO
					// impl builder

					// var message = BuildImpl<ODR>(x =>
				//	{ x.Set(y => y.CustomerId, customerId)
				//	}


					CurrentMessage.Respond(details);
				});


			PipelineViewer.Trace(bus.InboundPipeline);
		}
	}

	public class OrderDetailsReceivedImpl :
		OrderDetailsReceived
	{
		public OrderDetailsReceivedImpl(string customerId, string orderId, DateTime created, OrderStatus status)
		{
			CustomerId = customerId;
			OrderId = orderId;
			Created = created;
			Status = status;
		}

		protected OrderDetailsReceivedImpl()
		{
		}

		public string OrderId { get;  set; }
		public string CustomerId { get;  set; }
		public DateTime Created { get;  set; }
		public OrderStatus Status { get;  set; }
	}
}