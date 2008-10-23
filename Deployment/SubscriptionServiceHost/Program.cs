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
namespace SubscriptionServiceHost
{
    using System.IO;
    using Castle.Core;
    using log4net;
    using log4net.Config;
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            _log.Info("SubMgr Loading");

            var container = new DefaultMassTransitContainer("pubsub.castle.xml");
            container.AddComponentLifeStyle("followerrepository", typeof(FollowerRepository), LifestyleType.Singleton);

            container.AddComponent<IHostedService, SubscriptionService>();
            container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();

            //TODO: Polish this DB configuration option
            //Container.AddComponent<ISubscriptionRepository, NHibernateSubscriptionStorage>();   

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);
            var env = new SubscriptionServiceConfiguration(ServiceLocator.Current);


            Runner.Run(env, args);
        }
    }
}