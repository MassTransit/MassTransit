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
namespace TimeoutServiceHost
{
    using System.IO;
    using log4net;
    using log4net.Config;
    using MassTransit.Host;
    using MassTransit;
    using MassTransit.Services.Timeout;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            _log.Info("Timeout Service Loading");

            var container = new DefaultMassTransitContainer("timeout.castle.xml");
            container.AddComponent<ITimeoutRepository, InMemoryTimeoutRepository>();
            container.AddComponent<IHostedService, TimeoutService>();

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);

            var lifecycle = new TimeoutServiceLifeCycle(ServiceLocator.Current);
            var settings = WinServiceSettings.Custom("MT-TIMEOUT",
                                                     "Mass Transit Timeout Service",
                                                     "Think Egg Timer",
                                                     KnownServiceNames.Msmq);

            Runner.Run(Credentials.Interactive,
                settings, 
                lifecycle,
                args);
        }
    }
}
