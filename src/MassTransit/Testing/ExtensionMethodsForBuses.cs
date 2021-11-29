namespace MassTransit.Testing
{
    using System;
    using Implementations;


    public static class ExtensionMethodsForBuses
    {
        /// <summary>
        /// Creates a bus activity monitor
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IBusActivityMonitor CreateBusActivityMonitor(this IBus bus)
        {
            var sendIndicator = new BusActivitySendIndicator();
            var publishIndicator = new BusActivityPublishIndicator();
            var receiveIndicator = new BusActivityReceiveIndicator();
            var consumeIndicator = new BusActivityConsumeIndicator();

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
            var sendIndicator = new BusActivitySendIndicator(inactivityTimeout);
            var publishIndicator = new BusActivityPublishIndicator(inactivityTimeout);
            var receiveIndicator = new BusActivityReceiveIndicator(inactivityTimeout);
            var consumeIndicator = new BusActivityConsumeIndicator();

            return CreateBusActivityMonitorInternal(bus, receiveIndicator, consumeIndicator, sendIndicator, publishIndicator);
        }

        static IBusActivityMonitor CreateBusActivityMonitorInternal(IBus bus, BusActivityReceiveIndicator receiveIndicator,
            BusActivityConsumeIndicator consumeIndicator,
            BusActivitySendIndicator sendIndicator, BusActivityPublishIndicator publishIndicator)
        {
            var activityMonitor = new BusActivityMonitor();
            var conditionExpression = new ConditionExpression(activityMonitor);
            conditionExpression.AddConditionBlock(receiveIndicator, consumeIndicator, sendIndicator, publishIndicator);

            bus.ConnectReceiveObserver(receiveIndicator);
            bus.ConnectConsumeObserver(consumeIndicator);

            return activityMonitor;
        }
    }
}
