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
    using MassTransit;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Server;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;
    using Topshelf;
    using Topshelf.Configuration;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("healthService.log4net.xml"));
            _log.Info("Health Server Loading");

            var cfg = RunnerConfigurator.New(c=>
                                                 {
                                                     c.SetServiceName("MT-HEALTH");
                                                     c.SetDisplayName("MassTransit Health Manager");
                                                     c.SetDescription("Its like that machine at the hospital that goes beep beep, or errrrrrr... if you croak");

                                                     c.RunAsFromInteractive();

                                                     c.DependencyOnMsmq();

                                                     c.BeforeStart(a=>
                                                                       {
                                                                           var container = new DefaultMassTransitContainer("healthService.castle.xml");

                                                                           container.AddComponent<HealthService>();

                                                                           //TODO: Put database persitance here too

                                                                           var wob = new WindsorObjectBuilder(container.Kernel);
                                                                           ServiceLocator.SetLocatorProvider(() => wob);
                                                                       });

                                                     c.ConfigureService<HealthService>( s =>
                                                                                             {
                                                                                                 s.WhenStarted(tc => tc.Start());
                                                                                                 s.WhenStopped(tc => tc.Stop());
                                                                                                 s.WithName("Health service");
                                                                                             });
                                                 });
            Runner.Host(cfg, args);
        }
    }
}