// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Client
{
    using System.IO;
    using Castle.MicroKernel.Registration;
    using log4net;
    using log4net.Config;
    using MassTransit;
    using MassTransit.Configuration;
    using MassTransit.Services.Subscriptions.Configuration;
    using MassTransit.Transports.Msmq;
    using MassTransit.WindsorIntegration;
    using Topshelf;
    using Topshelf.Configuration;
    using Topshelf.Configuration.Dsl;

    internal class Program
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("client.log4net.xml"));
            _log.Info("Client Loading");


            var cfg = RunnerConfigurator.New(c =>
            {
                c.SetServiceName("SampleClientService");
                c.SetDisplayName("SampleClientService");
                c.SetDescription("SampleClientService");

                c.DependencyOnMsmq();
                c.RunAsLocalSystem();

                c.ConfigureService<ClientService>(s =>
                {
                    string serviceName = typeof (ClientService).Name;

                    s.Named(serviceName);
                    s.WhenStarted(o =>
                    {
                        MsmqEndpointConfigurator.Defaults(def =>
                        {
                            def.CreateMissingQueues = true;
                        });

                        var container = new DefaultMassTransitContainer("client.castle.xml");
                        var wob = new WindsorObjectBuilder(container.Kernel);

                        container.Register(Component.For<PasswordUpdater>());

                        Bus.Initialize(ec=>
                        {
                            ec.SetObjectBuilder(container.ObjectBuilder);
                            ec.RegisterTransport<MsmqEndpoint>();
                        },
                        bc=>
                        {
                            
                            bc.SetObjectBuilder(wob);
                            bc.ReceiveFrom("msmq://localhost/mt_client");
                            bc.ConfigureService<SubscriptionClientConfigurator>(client => client.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions"));
                        },
                        ()=>wob);
                        var bus = Bus.Instance();
                        o.Start(bus);
                    });
                    s.WhenStopped(o => o.Stop());

                    s.HowToBuildService(name => new ClientService());
                });
            });
            Runner.Host(cfg, args);
        }
    }
}