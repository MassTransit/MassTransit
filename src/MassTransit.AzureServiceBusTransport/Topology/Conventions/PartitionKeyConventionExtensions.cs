// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using AzureServiceBusTransport.Topology.Conventions;
    using AzureServiceBusTransport.Topology.Conventions.PartitionKey;
    using Topology.Configuration;


    public static class PartitionKeyConventionExtensions
    {
        public static void UsePartitionKeyFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, IMessagePartitionKeyFormatter<T> formatter)
            where T : class
        {
            configurator.UpdateConvention<IPartitionKeyMessageSendTopologyConvention<T>>(
                update =>
                {
                    update.SetFormatter(formatter);

                    return update;
                });
        }

        /// <summary>
        /// Use the partition key formatter for the specified message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UsePartitionKeyFormatter<T>(this ISendTopologyConfigurator configurator, IMessagePartitionKeyFormatter<T> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UsePartitionKeyFormatter(formatter);
        }

        /// <summary>
        /// Use the delegate to format the partition key, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UsePartitionKeyFormatter<T>(this ISendTopologyConfigurator configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.GetMessageTopology<T>().UsePartitionKeyFormatter(new DelegatePartitionKeyFormatter<T>(formatter));
        }

        /// <summary>
        /// Use the delegate to format the partition key, using Empty if the string is null upon return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="formatter"></param>
        public static void UsePartitionKeyFormatter<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<SendContext<T>, string> formatter)
            where T : class
        {
            configurator.UsePartitionKeyFormatter(new DelegatePartitionKeyFormatter<T>(formatter));
        }
    }
}