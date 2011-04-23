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

    public class MassTransitInstaller :
        IWindsorInstaller
    {
        readonly SettingsOptions _options;

        public MassTransitInstaller(SettingsOptions options)
        {
            _options = options;
        }

        public MassTransitInstaller()
        {
            _options = ConfigurationSection.GetSettings();
        }

        //we are expecting SM to auto-resolve
        // SubscriptionClient
        // InitiateSagaMessageSink<,>
        // OrchestrateSagaMessageSink<,>)
        // InitiateSagaStateMachineSink<,>)
        // OrchestrateSagaStateMachineSink<,>)
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var wob = new WindsorObjectBuilder(container.Kernel);

            container.Register(
                Component.For<IObjectBuilder>().Named("objectBuilder").Instance(wob).LifeStyle.Singleton
                );

            Bus.Initialize(wob, cfg =>
                {

                    cfg.CreateMissingQueues();
                    
                    cfg.AddTransportFactory(_options.Transports.Select(Type.GetType).ToArray());


                    cfg.ReceiveFrom(_options.ReceiveFrom);

                    

                    //if subscription service
                    if (_options.Subscriptions != null)
                    {
                        cfg.UseSubscriptionService(_options.Subscriptions);
                    }


                    //if management service
                    if (_options.HealthServiceInterval != null)
                    {
                        string mgmt = _options.HealthServiceInterval;
                        int interval = string.IsNullOrEmpty(mgmt) ? 60 : int.Parse(mgmt);
                        cfg.UseHealthMonitoring(interval);
                    }
                });


            container.Register(Component.For<IServiceBus>()
                .Named("serviceBus")
                .Instance(Bus.Instance())
                .LifeStyle.Singleton);

            container.Register(Component.For<IControlBus>()
                .Named("controlBus")
                .Instance((IControlBus) Bus.Instance().ControlBus)
                .LifeStyle.Singleton);


            container.Register(Component.For<IEndpointResolver>()
                .Named("endpointFactory")
                .Instance(Bus.Resolver())
                .LifeStyle.Singleton);
        }
    }
}