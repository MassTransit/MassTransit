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
    using Castle.Windsor;
    using log4net;
    using log4net.Config;
    using MassTransit;
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

            WindsorContainer container;

            RunConfiguration cfg = RunnerConfigurator.New(c =>
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
                        container = new WindsorContainer();
                        
                        container.Register(Component.For<PasswordUpdater>());

                        Bus.Initialize(sbc=>
                        {
                            sbc.ReceiveFrom("msmq://localhost/mt_client");
                            sbc.UseSubscriptionService("msmq://localhost/mt_subscriptions");
                            sbc.UseMsmq();
                            sbc.VerifyMsDtcConfiguration();
                            sbc.VerifyMsmqConfiguration();

                        });
                        var bus = Bus.Instance;
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