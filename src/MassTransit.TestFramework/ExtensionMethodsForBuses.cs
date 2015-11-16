namespace MassTransit.TestFramework
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