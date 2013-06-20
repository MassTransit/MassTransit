// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using Exceptions;
    using Services.Subscriptions.Configuration;
    using SubscriptionConfigurators;


    public static class SubscriptionClientConfiguratorExtensions
    {
        /// <summary>
        /// Sets the address of the Subscription Service which routes messages to multiple subscribers
        /// from a single publisher.  The address of the Subscription Service is used by both 
        /// publishers and subscribers, while routing is carried out by MassTransit.RuntimeServices
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="subscriptionServiceUri"></param>
        public static void UseSubscriptionService(this MsmqConfigurator configurator,
            string subscriptionServiceUri)
        {
            configurator.UseSubscriptionService(x => x.SetSubscriptionServiceEndpoint(subscriptionServiceUri.ToUri()));
        }

        /// <summary>
        /// Sets the address of the Subscription Service which routes messages to multiple subscribers
        /// from a single publisher.  The address of the Subscription Service is used by both 
        /// publishers and subscribers, while routing is carried out by MassTransit.RuntimeServices
        /// </summary>
        public static void UseSubscriptionService(this MsmqConfigurator configurator, Uri subscriptionServiceUri)
        {
            configurator.UseSubscriptionService(x => x.SetSubscriptionServiceEndpoint(subscriptionServiceUri));
        }

        /// <summary>
        /// Sets the address of the Subscription Service which routes messages to multiple subscribers
        /// from a single publisher.  The address of the Subscription Service is used by both 
        /// publishers and subscribers, while routing is carried out by MassTransit.RuntimeServices
        /// </summary>
        public static void UseSubscriptionService(this MsmqConfigurator configurator,
            Action<SubscriptionClientConfigurator> configureCallback)
        {
            var clientConfigurator = new SubscriptionClientConfiguratorImpl();

            configureCallback(clientConfigurator);

            var routerBuilderConfigurator = new SubscriptionRouterBuilderConfiguratorImpl(x => x.SetNetwork(null));

            configurator.Configurator.AddSubscriptionRouterConfigurator(routerBuilderConfigurator);

            configurator.Configurator.AddSubscriptionObserver(clientConfigurator.Create);
        }

        internal static Uri ToUri(this string uriString)
        {
            try
            {
                return new Uri(uriString);
            }
            catch (UriFormatException ex)
            {
                throw new ConfigurationException("The URI specified is invalid: " + uriString, ex);
            }
        }
    }
}