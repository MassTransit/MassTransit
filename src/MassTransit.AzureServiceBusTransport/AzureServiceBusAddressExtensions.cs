// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Linq;
    using Internals.Extensions;
    using Microsoft.ServiceBus.Messaging;


    public static class AzureServiceBusAddressExtensions
    {
        public static QueueDescription GetQueueDescription(this Uri address)
        {
            if (string.Compare("sb", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ArgumentException("The invalid scheme was specified: " + address.Scheme);

            string queueName = address.AbsolutePath.Split(new[] {'/'}).Last();

            var queueDescription = new QueueDescription(queueName)
            {
                EnableBatchedOperations = true,
                MaxDeliveryCount = 5,
                DefaultMessageTimeToLive = TimeSpan.FromDays(365),
                LockDuration = TimeSpan.FromMinutes(5),
                EnableDeadLetteringOnMessageExpiration = true,
            };

            queueDescription.DefaultMessageTimeToLive = address.GetValueFromQueryString("ttl", queueDescription.DefaultMessageTimeToLive);
            queueDescription.EnableExpress = address.GetValueFromQueryString("express", queueDescription.EnableExpress);

            return queueDescription;
        }
    }
}