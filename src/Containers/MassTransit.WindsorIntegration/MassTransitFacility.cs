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
namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Linq;
    using Services.HealthMonitoring.Configuration;
    using Services.Subscriptions.Configuration;
    using Transports;
    using Magnum.ObjectExtensions;

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
            base.FacilityConfig.Children
                .Where(x => x.Name == "bus")
                .Each(bus =>
            {
                var ep = bus.Attributes["endpoint"];

                //if subscription service
                var sub = bus.Children["subscriptionService"].Attributes["endpoint"];
                
                //if management service
                var mgmt = bus.Children["managementService"].Attributes["heartbeatInterval"];
                var interval = mgmt.IsNullOrEmpty() ? 60 : int.Parse(mgmt);

                base.RegisterServiceBus(ep,a=>
                {
                    if(!sub.IsNullOrEmpty() )a.ConfigureService<SubscriptionClientConfigurator>(s=>s.SetSubscriptionServiceEndpoint(sub));
                    a.ConfigureService<HealthClientConfigurator>(s=>s.SetHeartbeatInterval(interval));
                });
            });

            

            RegisterEndpointFactory(x =>
            {
                x.RegisterTransport<LoopbackEndpoint>();
                x.RegisterTransport<MulticastUdpEndpoint>();

                FacilityConfig.Children["transports"].Children
                .Where(n => n.Name == "transport")
                .Each(transport =>
                {
                    x.RegisterTransport(Type.GetType(transport.Value));
                });
            });
        }
    }
}