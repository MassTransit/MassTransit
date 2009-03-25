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
namespace DeferredMessageServiceHost
{
	using System.IO;
	using log4net;
	using log4net.Config;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Services.MessageDeferral;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Transports.Msmq;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("deferredMessageService.log4net.xml"));
			_log.Info("Deferred Message Service Loading");


			IRunConfiguration cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("MT-DEFERRED");
					c.SetDisplayName("MassTransit Deferred Message Service");
					c.SetDescription("Allows services to delay the handling of a message until a later time");

					c.RunAsFromInteractive();

					c.DependencyOnMsmq();
					c.DependsOn("MT-TIMEOUT");

					c.ConfigureService<MessageDeferralService>(s =>
						{
                            s.CreateServiceLocator(()=>
                                {
                                    var container = new DefaultMassTransitContainer();
                                    container.RegisterInMemorySagaRepository();
                                    var ef = EndpointFactoryConfigurator.New(e =>
                                    {
                                        e.RegisterTransport<MsmqEndpoint>();
                                    });
                                    container.Kernel.AddComponentInstance("endpointFactory", typeof(IEndpointFactory), ef);

                                    container.AddComponent<MessageDeferralService>();
                                    container.AddComponent<IDeferredMessageRepository, InMemoryDeferredMessageRepository>();
                                    //TODO: Put the Database Repository here too

                                    var bus = ServiceBusConfigurator.New(sbc =>
                                    {
                                        sbc.ReceiveFrom("msmq://localhost/mt_timeout");
                                        sbc.ConfigureService<SubscriptionClientConfigurator>(scc =>
                                        {
                                            scc.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions");
                                        });
                                    });

                                    container.Kernel.AddComponentInstance<IServiceBus>(bus);
                                    return container.ObjectBuilder;
                                });
							s.WhenStarted(tc =>
							    {
							        
							        tc.Start();
							    });
							s.WhenStopped(tc => tc.Stop());
							s.WithName("Deferred service");
						});
				});
			Runner.Host(cfg, args);
		}
	}
}