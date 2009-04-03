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
namespace HealthServiceHost
{
    using System.IO;
    using log4net;
    using log4net.Config;
    using MassTransit;
    using MassTransit.Configuration;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.Subscriptions.Configuration;
    using MassTransit.Transports.Msmq;
    using MassTransit.WindsorIntegration;
    using Topshelf;
    using Topshelf.Configuration;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("healthService.log4net.xml"));
            _log.Info("Health Server Loading");

            IRunConfiguration cfg = RunnerConfigurator.New(c =>
            {
                c.SetServiceName("MT-HEALTH");
                c.SetDisplayName("MassTransit Health Manager");
                c.SetDescription("Its like that machine at the hospital that goes beep beep, or errrrrrr... if you croak");

                c.RunAsFromInteractive();

                c.DependencyOnMsmq();

                c.ConfigureService<HealthService>(s =>
                {
                    s.CreateServiceLocator(() =>
                    {
                        var container = new DefaultMassTransitContainer();
                        container.RegisterInMemorySagaRepository();
                        IEndpointFactory ef = EndpointFactoryConfigurator.New(e => e.RegisterTransport<MsmqEndpoint>());
                        container.Kernel.AddComponentInstance("endpointFactory", typeof (IEndpointFactory), ef);

                        container.AddComponent<HealthService>();
                        IServiceBus bus = ServiceBusConfigurator.New(a =>
                        {
                            a.ReceiveFrom("msmq://localhost/mt_health");
                            a.ConfigureService<SubscriptionClientConfigurator>(sc => sc.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions"));
                        });
                        container.Kernel.AddComponentInstance<IServiceBus>(bus);
                        //TODO: Put database persitance here too

                        return container.ObjectBuilder;
                    });
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WithName("Health service");
                });
            });
            Runner.Host(cfg, args);
        }
    }
}