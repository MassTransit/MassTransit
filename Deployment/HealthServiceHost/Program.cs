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
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Services.HealthMonitoring;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.xml"));
            _log.Info("Health Server Loading");

            var container = new DefaultMassTransitContainer("health.castle.xml");

            
            container.AddComponent<IHostedService, HealthService>();
            container.AddComponent<IHealthCache, LocalHealthCache>();
            container.AddComponent<IHeartbeatTimer, InMemoryHeartbeatTimer>();
            //TODO: Put database persittance here too


            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);
            HealthServiceConfiguration cfg = new HealthServiceConfiguration(ServiceLocator.Current);

            Runner.Run(cfg, args);
        }
    }
}