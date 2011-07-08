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
namespace MassTransit.Configuration.Xml
{
    using System.Configuration;
    using System.Xml;

    /*
     * <masstransit>
     *  <ReceiveFrom>msmq://localhost/queuename</ReceiveFrom>
     *  <SubscriptionService>msmq://localhost/mt_subscriptions</SubscriptionService>
     *  <HealthService>3</HealthService>
     *  <Services> ?
     *  </Services>
     *  <Transports>
     *    <Transport>full assembly name</Transport>
     *  </Transports>
     * </masstransit>
     */

    public class ConfigurationSection :
        IConfigurationSectionHandler
    {
        public static SettingsOptions GetSettings()
        {
            var settings = (SettingsOptions) ConfigurationManager.GetSection("MassTransit");
            return settings;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var opts = new SettingsOptions();
            string receiveFrom = section.SelectSingleNode("ReceiveFrom").InnerText;
            opts.ReceiveFrom = receiveFrom.Trim();

            XmlNode transports = section.SelectSingleNode("Transports");
            foreach (XmlNode transport in transports.ChildNodes)
            {
                opts.Transports.Add(transport.InnerText.Trim());
            }

            var sub = section.SelectSingleNode("SubscriptionService");
            if (sub != null)
            {
                var s = sub.InnerText.Trim();
                opts.Subscriptions = s;
            }

            var health = section.SelectSingleNode("HealthService");
            if (health != null)
            {
                var h = health.InnerText.Trim();
                opts.HealthServiceInterval = h;
            }


            return opts;
        }
    }
}