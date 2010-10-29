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
namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Linq;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Configuration;
    using Configuration.Xml;
    using Services.HealthMonitoring.Configuration;
    using Services.Subscriptions.Configuration;

    //doesn't support configuration.
    //provide an XML config? 
    //it would be better to have one standard xml config
    public class MassTransitInstaller :
        IWindsorInstaller
    {
        //we are expecting SM to auto-resolve
        // SubscriptionClient
        // InitiateSagaMessageSink<,>
        // OrchestrateSagaMessageSink<,>)
        // InitiateSagaStateMachineSink<,>)
        // OrchestrateSagaStateMachineSink<,>)
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceBusConfigurator.Defaults(x => x.SetObjectBuilder(wob));
            container.Register(
                Component.For<IObjectBuilder>().Named("objectBuilder").Instance(wob).LifeStyle.Singleton
                );

            var xmlCfg = ConfigurationSection.GetSettings();
            Bus.Initialize(wob, (cfg, ep) =>
            {

                //this is because the init hasn't completed yet
                container.Register(Component.For<IEndpointFactory>()
                                       .Named("endpointFactory")
                                       .Instance(ep)
                                       .LifeStyle.Singleton);

                cfg.ReceiveFrom(xmlCfg.ReceiveFrom);


                //if subscription service
                if (xmlCfg.Subscriptions != null)
                {
                    cfg.UseSubscriptionService(xmlCfg.Subscriptions);
                }


                //if management service
                if (xmlCfg.HealthServiceInterval != null)
                {
                    string mgmt = xmlCfg.HealthServiceInterval;
                    int interval = string.IsNullOrEmpty(mgmt) ? 60 : int.Parse(mgmt);
                    cfg.UseHealthMonitoring(interval);
                }
            }, xmlCfg.Transports.Select(Type.GetType).ToArray());

            container.Register(Component.For<IServiceBus>()
                                   .Named("serviceBus")
                                   .Instance(Bus.Instance())
                                   .LifeStyle.Singleton);

            container.Register(Component.For<IControlBus>()
                                   .Named("controlBus")
                                   .Instance((IControlBus) Bus.Instance().ControlBus)
                                   .LifeStyle.Singleton);
        }
    }
}