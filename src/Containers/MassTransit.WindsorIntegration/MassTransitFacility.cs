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
    using Exceptions;
    using Services.HealthMonitoring.Configuration;
    using Services.Subscriptions.Configuration;

    public class MassTransitFacility :
        MassTransitFacilityBase
    {
        //    <facility id="masstransit">
        //      <bus id="customer"
        //				 endpoint="msmq://localhost/starbucks_customer">
        //        <subscriptionService endpoint="msmq://localhost/mt_subscriptions" />
        //        <managementService heartbeatInterval="3" />
        //      </bus>
        //      <transports>
        //        <transport>MassTransit.Transports.Msmq.MsmqEndpoint, MassTransit.Transports.Msmq</transport>
        //      </transports>
        //    </facility>
        protected override void Init()
        {
            if (FacilityConfig.Children.Where(n => n.Name == "bus").Count() > 1)
                throw new ConfigurationException("You can only have one bus node in the castle xml in the 'MassTransit' facility");


            Bus.Initialize(cfg =>
            {
                var bus = FacilityConfig.Children["bus"];

                var ep = bus.Attributes["endpoint"];
                cfg.ReceiveFrom(ep);

                //if subscription service
                if (bus.Children["subscriptionService"] != null)
                {
                    var sub = bus.Children["subscriptionService"].Attributes["endpoint"];
                    cfg.UseSubscriptionService(sub);
                }

                FacilityConfig.Children["transports"].Children
                    .Where(n => n.Name == "transport")
                    .Each(transport => cfg.RegisterTransport(Type.GetType(transport.Value)));

                //if management service
                if (bus.Children["managementService"] != null)
                {
                    var mgmt = bus.Children["managementService"].Attributes["heartbeatInterval"];
                    var interval = string.IsNullOrEmpty(mgmt) ? 60 : int.Parse(mgmt);
                    cfg.UseHealthMonitoring(interval);
                }
            }, () => Kernel.Resolve<IObjectBuilder>());

            Kernel.Register(Component.For<IServiceBus>()
                                .Named("serviceBus")
                                .Instance(Bus.Instance())
                                .LifeStyle.Singleton);

            Kernel.Register(Component.For<IControlBus>()
                                .Named("controlBus")
                                .Instance((IControlBus) Bus.Instance().ControlBus)
                                .LifeStyle.Singleton);
            Kernel.Register(Component.For<IEndpointFactory>()
                                .Named("endpointFactory")
                                .Instance(Bus.Factory())
                                .LifeStyle.Singleton);

        }
    }
}