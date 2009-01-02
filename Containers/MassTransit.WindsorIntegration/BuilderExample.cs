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
namespace MassTransit.WindsorIntegration
{
    using System;
    using Configuration;
	using Serialization;
	using Transports;

	public class BuilderExample
	{
		public BuilderExample()
		{
			const IObjectBuilder objectBuilder = null;

			IEndpointFactory endpointFactory = EndpointFactoryConfigurator.New(x =>
				{
					x.SetDefaultSerializer<BinaryMessageSerializer>();
					x.RegisterTransport<LoopbackEndpoint>();

					x.ConfigureEndpoint("loopback://localhost/mt_client_control", y =>
						{
							y.SetSerializer<XmlMessageSerializer>();
						});
				});

			ServiceBusConfigurator.Defaults(def =>
				{
					def.SetObjectBuilder(objectBuilder);
					def.SetReceiveTimeout(TimeSpan.FromSeconds(1));
					def.SetConcurrentConsumerLimit(20);
					def.SetConcurrentReceiverLimit(1);
				});

			IServiceBus bus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom("msmq://localhost/mt_client");

					x.SetObjectBuilder(objectBuilder);
					x.SetConcurrentConsumerLimit(20);
					x.SendErrorsTo("msmq://localhost/mt_client_errors");

					// create a shim between the main ServiceBus and the child service bus that is really constrained
					// to provide simple control interface to the services on the bus
//
//					x.SetControlBus(c => c.ReceiveFrom("msmq://localhost/mt_client_control"));
//
//					x.ConfigureServices(services =>
//						{
//							SubscriptionClientConfigurator.New(y =>
//								{
//									y.SetSubscriptionServiceEndpoint("");
//
//								});
//						});
//


					x.ConfigureService<SubscriptionClientConfigurator>(y => y.SetEndpoint("msmq://localhost/mt_pubsub"));
					//x.ConfigureService<HealthClientConfigurator>(y => y.SetEndpoint("msmq://localhost/mt_pubsub"));

					x.EnableAutoSubscribe();
					x.DisableAutoStart();
				});


		//	bus.ConfigureInboundPipeline(x => { x.Filter<object>(new MessageWireTap("msmq://localhost/message_copy")); });


			var simplestBus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom("msmq://localhost/mt_server");
					x.SetObjectBuilder(objectBuilder);
				});
		}
	}

}