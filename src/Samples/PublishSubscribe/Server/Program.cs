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
namespace Server
{
    using System.IO;
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
            XmlConfigurator.ConfigureAndWatch(new FileInfo("server.log4net.xml"));
            _log.Info("Server Loading");

            RunConfiguration cfg = RunnerConfigurator.New(c =>
            {
                c.SetServiceName("SampleService");
                c.SetServiceName("Sample Service");
                c.SetServiceName("Something");
                c.DependencyOnMsmq();

                MsmqEndpointConfigurator.Defaults(def =>
                {
                    def.CreateMissingQueues = true;
                });

                c.ConfigureService<PasswordUpdateService>(s =>
                {
                    s.WhenStarted(o =>
                    {
                        var container = new DefaultMassTransitContainer("server.castle.xml");
                        var wob = new WindsorObjectBuilder(container.Kernel);

                        var endpointFactory = EndpointFactoryConfigurator.New(e =>
                        {
                            e.SetObjectBuilder(wob);
                            e.RegisterTransport<MsmqEndpoint>();
                        });

                        Bus.Initialize(ec =>
                        {
                            ec.SetObjectBuilder(wob);
                            ec.RegisterTransport<MsmqEndpoint>();
                        },
                                       bc =>
                                       {
                                           bc.SetObjectBuilder(wob);
                                           bc.ReceiveFrom("msmq://localhost/mt_server");
                                           bc.ConfigureService<SubscriptionClientConfigurator>(b => b.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions"));
                                       },
                                       () => wob);
                        IServiceBus b2 = Bus.Instance();
                        o.Start(b2);
                    });
                    s.WhenStopped(o => o.Stop());

                    s.HowToBuildService(name => new PasswordUpdateService());
                });
            });
            Runner.Host(cfg, args);
        }
    }
}