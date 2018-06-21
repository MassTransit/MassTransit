// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing
{
    using System;
    using Indicators;


    public static class ExtensionMethodsForBuses
    {

        /// <summary>
        /// Creates a bus activity monitor
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IBusActivityMonitor CreateBusActivityMonitor(this IBus bus)
        {
            BusActivitySendIndicator sendIndicator = new BusActivitySendIndicator();
            BusActivityPublishIndicator publishIndicator = new BusActivityPublishIndicator();
            BusActivityReceiveIndicator receiveIndicator = new BusActivityReceiveIndicator();
            BusActivityConsumeIndicator consumeIndicator = new BusActivityConsumeIndicator();

            return CreateBusActivityMonitorInternal(bus, receiveIndicator, consumeIndicator, sendIndicator, publishIndicator);
        }

        /// <summary>
        /// Creates a bus activity monitor
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="inactivityTimeout">minimum time to wait to presume bus inactivity</param>
        /// <returns></returns>
        public static IBusActivityMonitor CreateBusActivityMonitor(this IBus bus, TimeSpan inactivityTimeout)
        {
            BusActivitySendIndicator sendIndicator = new BusActivitySendIndicator(inactivityTimeout);
            BusActivityPublishIndicator publishIndicator = new BusActivityPublishIndicator(inactivityTimeout);
            BusActivityReceiveIndicator receiveIndicator = new BusActivityReceiveIndicator(inactivityTimeout);
            BusActivityConsumeIndicator consumeIndicator = new BusActivityConsumeIndicator();

            return CreateBusActivityMonitorInternal(bus, receiveIndicator, consumeIndicator, sendIndicator, publishIndicator);
        }

        static IBusActivityMonitor CreateBusActivityMonitorInternal(IBus bus, BusActivityReceiveIndicator receiveIndicator, BusActivityConsumeIndicator consumeIndicator,
            BusActivitySendIndicator sendIndicator, BusActivityPublishIndicator publishIndicator)
        {
            BusActivityMonitor activityMonitor = new BusActivityMonitor();
            ConditionExpression conditionExpression = new ConditionExpression(activityMonitor);
            conditionExpression.AddConditionBlock(receiveIndicator, consumeIndicator, sendIndicator, publishIndicator);

            bus.ConnectReceiveObserver(receiveIndicator);
            bus.ConnectConsumeObserver(consumeIndicator);

            return activityMonitor;
        }
    }
}